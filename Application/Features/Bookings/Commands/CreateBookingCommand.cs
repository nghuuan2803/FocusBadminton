using Application.Common;
using Application.Interfaces;
using Application.Models.PaymentModels;
using AutoMapper;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Shared.Bookings;
using Shared.CostCalculators;
using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Bookings.Commands
{
    public class CreateBookingCommand : IRequest<Result<BookingDTO>>
    {
        public int MemberId { get; set; }
        public int? TeamId { get; set; }
        public BookingType Type { get; set; } = BookingType.InDay;
        public DateTimeOffset? ApprovedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public double Amount { get; set; }
        public double Deposit { get; set; }
        public int? VoucherId { get; set; }
        public double Discount { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public string? TransactionImage { get; set; }

        [MaxLength(250)]
        public string? Note { get; set; }
        [MaxLength(250)]
        public string? AdminNote { get; set; }

        public ICollection<BookingItem>? Details { get; set; }
    }

    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Result<BookingDTO>>
    {
        private readonly IRepository<Booking> _repository;
        private readonly IRepository<Payment> _paymentRepo;
        private readonly IRepository<BookingHold> _holdRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISlotNotification _slotNotification;
        private readonly ICostCalculatorFactory _costCalculatorFactory; // Thay BookingCostCalculator bằng ICostCalculatorFactory
        private readonly IPaymentAdapterFactory _paymentAdapterFactory;
        private readonly Logger _logger;
        private readonly IServiceProvider _provider;

        public CreateBookingCommandHandler(
           IServiceProvider provider)
        {
            _provider = provider;
            _repository = provider.GetRequiredService<IRepository<Booking>>();
            _holdRepo = provider.GetRequiredService<IRepository<BookingHold>>();
            _paymentRepo = provider.GetRequiredService<IRepository<Payment>>();
            _unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            _mapper = provider.GetRequiredService<IMapper>();
            _slotNotification = provider.GetRequiredService<ISlotNotification>();
            _costCalculatorFactory = provider.GetRequiredService<ICostCalculatorFactory>();
            _paymentAdapterFactory = provider.GetRequiredService<IPaymentAdapterFactory>();
            _logger = Logger.Instance;
        }

        public async Task<Result<BookingDTO>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            _logger.Log($"Bắt đầu xử lý CreateBookingCommand cho MemberId: {request.MemberId}");
            if (!request.Details?.Any() ?? true)
            {
                return Result<BookingDTO>.Failure(Error.Validation("Chưa chọn sân"));
            }

            // Kiểm tra BookingHold
            _logger.Log($"Kiểm tra BookingHolds cho MemberId: {request.MemberId}, Số lượng khung giờ yêu cầu: {request.Details.Count}");
            var holds = await _holdRepo.GetAllAsync(x => x.HeldBy == request.MemberId.ToString() && x.ExpiresAt > DateTimeOffset.UtcNow);
            if (holds.Count() < request.Details.Count)
            {
                _logger.Log($"Thất bại: Số lượng BookingHolds ({holds.Count()}) nhỏ hơn số lượng khung giờ yêu cầu ({request.Details.Count})");
                return Result<BookingDTO>.Failure(Error.Validation("Chưa giữ lịch"));
            }

            foreach (var item in request.Details)
            {
                if (!holds.Any(x => x.CourtId == item.CourtId &&
                                    x.TimeSlotId == item.TimeSlotId &&
                                    x.BeginAt == item.BeginAt &&
                                    x.EndAt == item.EndAt &&
                                    x.DayOfWeek == item.DayOfWeek))
                {
                    _logger.Log($"Thất bại: Không tìm thấy BookingHold khớp với CourtId: {item.CourtId}, TimeSlotId: {item.TimeSlotId}, BeginAt: {item.BeginAt}, EndAt: {item.EndAt}, DayOfWeek: {item.DayOfWeek}");
                    return Result<BookingDTO>.Failure(Error.Validation("Chưa giữ lịch"));
                }
            }
            _logger.Log($"Kiểm tra BookingHolds thành công, tìm thấy {holds.Count()} bản ghi khớp với yêu cầu");

            // Bắt đầu transaction
            _logger.Log("Bắt đầu transaction để tạo Booking");
            await _unitOfWork.BeginAsync();

            try
            {
                // Tạo booking
                _logger.Log("Tạo Booking mới từ CreateBookingCommand");
                var booking = _mapper.Map<Booking>(request);

                // Xóa giữ lịch
                _logger.Log($"Xóa {holds.Count()} bản ghi BookingHolds");
                _holdRepo.RemoveRange(holds);

                // Tính giá cho từng BookingItem và tổng Amount
                double totalAmount = 0;
                double finalAmount = 0;
                foreach (var detail in booking.Details)
                {
                    var costRequest = new CostCalculatorRequest
                    {
                        CourtId = detail.CourtId,
                        TimeSlotId = detail.TimeSlotId,
                        BeginAt = detail.BeginAt.Value,
                        EndAt = detail.EndAt,
                        DayOfWeek = detail.DayOfWeek,
                        MemberId = request.MemberId,
                        VoucherId = request.VoucherId,
                        PromotionId = null, // Có thể thêm PromotionId vào CreateBookingCommand nếu cần
                        BookingType = request.Type
                    };
                    var totalCostCalculator = new BaseCostCalculator(_provider);
                    var finalCostCalculator = _costCalculatorFactory.CreateCalculator(costRequest);
                    double itemBaseCost = await totalCostCalculator.CalculateAsync(costRequest);
                    double itemCost = await finalCostCalculator.CalculateAsync(costRequest);
                    if (itemCost < 0)
                    {
                        throw new Exception($"Không thể tính giá cho CourtId: {detail.CourtId}, TimeSlotId: {detail.TimeSlotId}");
                    }
                    totalAmount += itemBaseCost;
                    finalAmount += itemCost;
                }
                booking.Amount = totalAmount;
                double discount = totalAmount - finalAmount;
                booking.Discount = discount < 0 ? 0 : discount;
                // Lưu booking
                _logger.Log("Lưu Booking vào cơ sở dữ liệu");
                await _repository.AddAsync(booking);
                await _repository.SaveAsync();

                // Tạo Payment dựa trên PaymentMethod
                string paymentUrl = string.Empty;
                var payment = new Payment
                {
                    BookingId = booking.Id,
                    Method = request.PaymentMethod,
                    Amount = request.Deposit > 0 ? request.Deposit : booking.Amount, // Ưu tiên deposit nếu có
                    Type = request.Deposit > 0 ? PaymentType.Deposit : PaymentType.CompleteBooking,
                    Status = PaymentStatus.Pending
                };

                switch (request.PaymentMethod)
                {
                    case PaymentMethod.Cash:
                        payment.Status = PaymentStatus.Pending; // Chờ xác nhận thủ công
                        //cần cập nhật lại entity Payment: cho phép null, kiểu dữ liệu DatetimeOffset
                        payment.PaidAt = DateTime.Now; // Chưa thanh toán
                        //payment.PaidAt = null; // Chưa thanh toán
                        break;

                    case PaymentMethod.BankTransfer:
                        payment.Image = request.TransactionImage; // Lưu ảnh chuyển khoản
                        payment.Status = PaymentStatus.Pending; // Chờ duyệt
                        break;

                    case PaymentMethod.Momo:
                    case PaymentMethod.VnPay:
                        var adapter = _paymentAdapterFactory.CreateAdapter(request.PaymentMethod);
                        var paymentRequest = new PaymentRequest
                        {
                            FullName = "User", // Lấy từ Member nếu cần
                            OrderId = booking.Id.ToString(),
                            OrderInfo = $"Thanh toan booking {booking.Id}",
                            Amount = payment.Amount,
                            Action = "mobile" // Dành cho Flutter
                        };
                        var paymentLink = await adapter.GeneratePaymentLinkAsync(paymentRequest);
                        if (!paymentLink.IsSuccess)
                        {
                            throw new Exception($"Failed to create payment link: {paymentLink.ErrorMessage}");
                        }
                        paymentUrl = paymentLink.Deeplink;
                        //payment.Info = paymentLink.Deeplink ?? paymentLink.PaymentUrl; // Lưu deeplink hoặc URL
                        break;
                }

                await _paymentRepo.AddAsync(payment);

                // Commit transaction
                _logger.Log("Commit transaction");
                await _unitOfWork.CommitAsync();

                // Tạo payload chi tiết cho NotifyBookingCreatedAsync
                var payload = new
                {
                    BookingId = booking.Id,
                    Status = (int)booking.Status,
                    BookingType = (int)booking.Type,
                    BookBy = request.MemberId.ToString(),
                    Details = booking.Details?.Select(d => new
                    {
                        CourtId = d.CourtId,
                        TimeSlotId = d.TimeSlotId,
                        BeginAt = d.BeginAt.Value.ToOffset(TimeSpan.FromHours(7)),
                        EndAt = d.EndAt != null ? ((DateTimeOffset)d.EndAt).ToOffset(TimeSpan.FromHours(7)) : default,
                        DayOfWeek = d.DayOfWeek
                    }).ToList()
                };
                _logger.Log($"Gửi thông báo NotifyBookingCreatedAsync với BookingId: {booking.Id}, Status: {(int)booking.Status}, BookBy: {request.MemberId}");

                // Gửi thông báo cho client
                await _slotNotification.NotifyBookingCreatedAsync(payload, cancellationToken);
                _logger.Log("Thông báo NotifyBookingCreatedAsync đã được gửi thành công");

                // Trả về kết quả
                var result = _mapper.Map<BookingDTO>(booking);
                result.PaymentLink = paymentUrl;
                _logger.Log($"Tạo Booking thành công, trả về BookingDTO với BookingId: {result.Id}");
                return Result<BookingDTO>.Success(result);
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi
                await _unitOfWork.RollbackAsync();
                _logger.Log($"Lỗi khi tạo Booking: {ex.Message}. Transaction đã được rollback");
                return Result<BookingDTO>.Failure(Error.UnknowError(ex.Message));
            }
        }
    }
}
