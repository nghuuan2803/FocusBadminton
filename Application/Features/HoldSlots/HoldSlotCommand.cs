using Application.Interfaces.DapperQueries;
using Domain.Repositories;
using Shared.Enums;

namespace Application.Features.HoldSlots
{
    public class HoldSlotCommand : IRequest<int>
    {
        public int CourtId { get; set; }
        public int TimeSlotId { get; set; }
        public DateTime HoldAt { get; set; }
        public string? HoldBy { get; set; }
        public BookingType BookingType { get; set; }
        public DateTimeOffset BeginAt { get; set; }
        public DateTimeOffset? EndAt { get; set; }
        public string? DayOfWeek { get; set; }
    }

    public class HoldSlotCommandHandler : IRequestHandler<HoldSlotCommand, int>
    {
        private readonly IRepository<BookingHold> _repository;
        private readonly IScheduleQueries _schedules;

        public HoldSlotCommandHandler(IRepository<BookingHold> repository, IScheduleQueries schedules)
        {
            _repository = repository;
            _schedules = schedules;
        }

        public async Task<int> Handle(HoldSlotCommand request, CancellationToken cancellationToken)
        {
            var available = await _schedules.CheckAvailable(request.CourtId,
                request.TimeSlotId,
                request.BookingType,
                request.BeginAt,
                request.EndAt,
                request.DayOfWeek ?? request.BeginAt.DayOfWeek.ToString());

            if (!available)
            {
                return 0;
            }
            var bookingHold = new BookingHold
            {
                CourtId = request.CourtId,
                TimeSlotId = request.TimeSlotId,
                HeldAt = request.HoldAt,
                HeldBy = request.HoldBy,
                BookingType = request.BookingType,
                BeginAt = request.BeginAt,
                EndAt = request.EndAt,
                DayOfWeek = request.DayOfWeek
            };
            await _repository.AddAsync(bookingHold,cancellationToken);
            await _repository.SaveAsync(cancellationToken);
            return bookingHold.Id;
        }
    }
}
