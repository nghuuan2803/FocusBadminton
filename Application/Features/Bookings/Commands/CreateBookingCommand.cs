using Application.Interfaces;
using AutoMapper;
using Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Shared.Bookings;
using Shared.Enums;

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
        public string? Note { get; set; }
        public string? AdminNote { get; set; }

        public ICollection<BookingItem>? Details { get; set; }
    }

    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Result<BookingDTO>>
    {
        private readonly IRepository<Booking> _repository;
        private readonly IRepository<BookingHold> _holdRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISlotNotification _slotNotification;
        private readonly PaymentHandler _paymentHandler;
        private readonly BookingCostProcessor _costProcessor;
        private readonly Logger _logger;

        public CreateBookingCommandHandler(
            IServiceProvider provider)
        {
            var scope = provider.CreateScope();
            var sp = scope.ServiceProvider;

            _repository = sp.GetRequiredService<IRepository<Booking>>();
            _holdRepo = sp.GetRequiredService<IRepository<BookingHold>>();
            _unitOfWork = sp.GetRequiredService<IUnitOfWork>();
            _mapper = sp.GetRequiredService<IMapper>();
            _slotNotification = sp.GetRequiredService<ISlotNotification>();
            _paymentHandler = new PaymentHandler(
                sp.GetRequiredService<IRepository<Payment>>(),
                sp.GetRequiredService<IPaymentAdapterFactory>(),
                Logger.Instance);
            _costProcessor = new BookingCostProcessor(
                sp,
                sp.GetRequiredService<ICostCalculatorFactory>());
            _logger = Logger.Instance;
        }

        public async Task<Result<BookingDTO>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            _logger.Log($"Bắt đầu xử lý CreateBookingCommand cho MemberId: {request.MemberId}");

            if (!request.Details?.Any() ?? true)
                return Result<BookingDTO>.Failure(Error.Validation("Chưa chọn sân"));

            // Kiểm tra BookingHold
            var holds = await _holdRepo.GetAllAsync(x => x.HeldBy == request.MemberId.ToString() &&
                                                        x.ExpiresAt > DateTimeOffset.UtcNow);
            if (holds.Count() < request.Details.Count ||
                !request.Details.All(item => holds.Any(x =>
                    x.CourtId == item.CourtId &&
                    x.TimeSlotId == item.TimeSlotId &&
                    x.BeginAt == item.BeginAt &&
                    x.EndAt == item.EndAt &&
                    x.DayOfWeek == item.DayOfWeek)))
            {
                return Result<BookingDTO>.Failure(Error.Validation("Chưa giữ lịch"));
            }

            await _unitOfWork.BeginAsync();
            try
            {
                var booking = _mapper.Map<Booking>(request);
                _holdRepo.RemoveRange(holds);

                // Tính toán chi phí
                var (totalAmount, discount) = await _costProcessor.CalculateBookingCostAsync(booking, request);
                booking.Amount = totalAmount;
                booking.Discount = discount;

                await _repository.AddAsync(booking);
                await _repository.SaveAsync();

                // Xử lý payment
                var (payment, paymentUrl) = await _paymentHandler.ProcessPaymentAsync(booking, request);

                await _unitOfWork.CommitAsync();

                // Gửi thông báo
                var payload = new
                {
                    BookingId = booking.Id,
                    Status = (int)booking.Status,
                    BookingType = (int)booking.Type,
                    BookBy = request.MemberId.ToString(),
                    Details = booking.Details?.Select(d => new
                    {
                        d.CourtId,
                        d.TimeSlotId,
                        BeginAt = d.BeginAt.Value.ToOffset(TimeSpan.FromHours(7)),
                        EndAt = d.EndAt != null ? ((DateTimeOffset)d.EndAt).ToOffset(TimeSpan.FromHours(7)) : default,
                        d.DayOfWeek
                    }).ToList()
                };
                await _slotNotification.NotifyBookingCreatedAsync(payload, cancellationToken);

                var result = _mapper.Map<BookingDTO>(booking);
                result.PaymentLink = paymentUrl;
                return Result<BookingDTO>.Success(result);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return Result<BookingDTO>.Failure(Error.UnknowError(ex.Message));
            }
        }
    }
}
