﻿using Application.Interfaces;
using Application.Interfaces.DapperQueries;
using Domain.Repositories;
using Shared.CostCalculators;
using Shared.Enums;
using Shared.Slots;

namespace Application.Features.Slots
{
    public class HoldSlotCommand : IRequest<HoldSlotResult>
    {
        public int CourtId { get; set; }
        public int TimeSlotId { get; set; }
        public string? HoldBy { get; set; }
        public BookingType BookingType { get; set; }
        public DateTimeOffset BeginAt { get; set; }
        public DateTimeOffset? EndAt { get; set; }
        public string? DayOfWeek { get; set; }
    }

    public class HoldSlotCommandHandler : IRequestHandler<HoldSlotCommand, HoldSlotResult>
    {
        private readonly IRepository<BookingHold> _repository;
        private readonly IScheduleQueries _schedules;
        private readonly ISlotNotification _slotNotification;
        private readonly ICostCalculatorFactory _costCalculatorFactory;

        public HoldSlotCommandHandler(
            IRepository<BookingHold> repository,
            IScheduleQueries schedules,
            ICostCalculatorFactory costCalculatorFactory,
            ISlotNotification slotNotification)
        {
            _repository = repository;
            _schedules = schedules;
            _slotNotification = slotNotification;
            _costCalculatorFactory = costCalculatorFactory;
        }

        public async Task<HoldSlotResult> Handle(HoldSlotCommand request, CancellationToken cancellationToken)
        {
            // Kiểm tra slot có khả dụng để giữ hay không
            var available = await _schedules.CheckAvailable(
                request.CourtId,
                request.TimeSlotId,
                request.BookingType,
                request.BeginAt.ToUniversalTime(),
                request.EndAt == null ? null : request.EndAt.Value.ToUniversalTime(),
                request.DayOfWeek);

            if (!available)
            {
                return null;
            }

            var now = DateTimeOffset.UtcNow;
            var bookingHold = new BookingHold
            {
                CourtId = request.CourtId,
                TimeSlotId = request.TimeSlotId,
                HeldAt = now,
                HeldBy = request.HoldBy,
                ExpiresAt = now.AddMinutes(5),
                BookingType = request.BookingType,
                BeginAt = request.BeginAt,
                EndAt = request.EndAt,
                DayOfWeek = request.DayOfWeek
            };

            // Lưu record BookingHold vào repository (và database)
            await _repository.AddAsync(bookingHold, cancellationToken);
            await _repository.SaveAsync(cancellationToken);

            // Tạo payload chi tiết chứa các thông tin cần thiết để client cập nhật giao diện và xử lý logic tránh overlap
            var payload = new
            {
                HoldSlotId = bookingHold.Id,
                bookingHold.CourtId,
                bookingHold.TimeSlotId,
                BookingType = (int)bookingHold.BookingType,
                bookingHold.BeginAt,
                bookingHold.EndAt,
                bookingHold.DayOfWeek,
                bookingHold.HeldBy
            };
            CostCalculatorRequest costRequest = new CostCalculatorRequest
            {
                CourtId = bookingHold.CourtId,
                TimeSlotId = bookingHold.TimeSlotId,
                BeginAt = bookingHold.BeginAt.Value,
                EndAt = bookingHold.EndAt,
                DayOfWeek = bookingHold.DayOfWeek ?? string.Empty,
            };
            var calculator = _costCalculatorFactory.CreateCalculator(costRequest);
            double estimatedCost = await calculator.CalculateAsync(costRequest);
            // Gửi thông báo cho tất cả client rằng slot đã được giữ với payload chi tiết
            await _slotNotification.NotifySlotHeldAsync(payload, cancellationToken);

            return new HoldSlotResult
            {
                HoldId = bookingHold.Id,
                DayOfWeek = bookingHold.DayOfWeek,
                EstimatedCost = estimatedCost
            };
        }
    }
}
