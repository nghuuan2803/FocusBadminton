using Application.Interfaces.DapperQueries;
using Application.Interfaces;
using Domain.Repositories;
using Shared.Enums;
using Shared.Slots;
using Shared.CostCalculators;

namespace Application.Features.Slots
{
    public class FixedBookingHoldCommand : IRequest<Result<List<HoldSlotResult>>>
    {
        public int CourtId { get; set; }
        public int TimeSlotId { get; set; }
        public DateTimeOffset BeginAt { get; set; }
        public DateTimeOffset? EndAt { get; set; }
        public List<string> DaysOfWeek { get; set; } = new List<string>();
        public BookingType BookingType { get; set; } = BookingType.Fixed;
        public string HeldBy { get; set; } = string.Empty;
    }
    public class FixedBookingHoldCommandHandler : IRequestHandler<FixedBookingHoldCommand, Result<List<HoldSlotResult>>>
    {
        private readonly IRepository<BookingHold> _repository;
        private readonly IScheduleQueries _schedules;
        private readonly ISlotNotification _slotNotification;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Logger _logger;
        ICostCalculatorFactory _calculatorFactory;

        public FixedBookingHoldCommandHandler(
            IRepository<BookingHold> repository,
            IScheduleQueries schedules,
            ISlotNotification slotNotification,
            IUnitOfWork unitOfWork,
            ICostCalculatorFactory calculatorFactory)
        {
            _repository = repository;
            _schedules = schedules;
            _slotNotification = slotNotification;
            _unitOfWork = unitOfWork;
            _logger = Logger.Instance;
            _calculatorFactory = calculatorFactory;
        }

        public async Task<Result<List<HoldSlotResult>>> Handle(FixedBookingHoldCommand request, CancellationToken cancellationToken)
        {
            _logger.Log($"Bắt đầu xử lý CheckFixedBookingHoldCommand cho CourtId: {request.CourtId}, TimeSlotId: {request.TimeSlotId}");

            if (!request.DaysOfWeek.Any())
            {
                return Result<List<HoldSlotResult>>.Failure(Error.Validation("Chưa chọn ngày trong tuần"));
            }

            var holdResults = new List<HoldSlotResult>();
            var now = DateTimeOffset.UtcNow;

            await _unitOfWork.BeginAsync();

            try
            {
                foreach (var dayOfWeek in request.DaysOfWeek)
                {
                    _logger.Log($"Kiểm tra availability cho {dayOfWeek}");
                    var available = await _schedules.CheckAvailable(
                        request.CourtId,
                        request.TimeSlotId,
                        request.BookingType,
                        request.BeginAt.ToUniversalTime(),
                        request.EndAt?.ToUniversalTime(),
                        dayOfWeek);

                    if (!available)
                    {
                        _logger.Log($"Không thể giữ lịch cho {dayOfWeek}: Không khả dụng");
                        await _unitOfWork.RollbackAsync();
                        return Result<List<HoldSlotResult>>.Failure(Error.Validation($"Không thể giữ lịch cho {dayOfWeek} do không khả dụng"));
                    }

                    var bookingHold = new BookingHold
                    {
                        CourtId = request.CourtId,
                        TimeSlotId = request.TimeSlotId,
                        HeldAt = now,
                        HeldBy = request.HeldBy,
                        ExpiresAt = now.AddMinutes(5),
                        BookingType = request.BookingType,
                        BeginAt = request.BeginAt,
                        EndAt = request.EndAt,
                        DayOfWeek = dayOfWeek,
                    };

                    // tạm tính giá
                    var costCalReq = new CostCalculatorRequest
                    {
                        CourtId = request.CourtId,
                        TimeSlotId = request.TimeSlotId,
                        BeginAt = request.BeginAt,
                        EndAt = request.EndAt,
                        DayOfWeek = dayOfWeek,
                        MemberId = int.Parse(request.HeldBy),
                        BookingType = BookingType.Fixed
                    };
                    var calculator = _calculatorFactory.CreateCalculator(costCalReq);
                    double estimatedCost = await calculator.CalculateAsync(costCalReq);
                    // Thêm và lấy ID từ repository
                    await _repository.AddAsync(bookingHold, cancellationToken);
                    await _repository.SaveAsync(cancellationToken); // Lưu để lấy ID

                    // Đảm bảo bookingHold.Id đã được cập nhật
                    if (bookingHold.Id <= 0)
                    {
                        throw new Exception($"Không thể lấy ID cho BookingHold của {dayOfWeek}");
                    }

                    holdResults.Add(new HoldSlotResult { DayOfWeek = dayOfWeek, HoldId = bookingHold.Id, EstimatedCost = estimatedCost });
                    _logger.Log($"Đã tạo BookingHold cho {dayOfWeek}, HoldId: {bookingHold.Id}");
                }

                await _unitOfWork.CommitAsync();

                foreach (var result in holdResults)
                {
                    var payload = new
                    {
                        HoldSlotId = result.HoldId,
                        CourtId = request.CourtId,
                        TimeSlotId = request.TimeSlotId,
                        BookingType = (int)request.BookingType,
                        BeginAt = request.BeginAt,
                        EndAt = request.EndAt,
                        DayOfWeek = result.DayOfWeek,
                        HeldBy = request.HeldBy
                    };
                    await _slotNotification.NotifySlotHeldAsync(payload, cancellationToken);
                    _logger.Log($"Đã gửi thông báo NotifySlotHeldAsync cho HoldId: {result.HoldId}");
                }

                _logger.Log($"Hoàn tất: Đã giữ {holdResults.Count} slot cho các ngày {string.Join(", ", request.DaysOfWeek)}");
                return Result<List<HoldSlotResult>>.Success(holdResults);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                _logger.Log($"Lỗi khi xử lý CheckFixedBookingHoldCommand: {ex.Message}");
                return Result<List<HoldSlotResult>>.Failure(Error.UnknowError(ex.Message));
            }
        }
    }
}
