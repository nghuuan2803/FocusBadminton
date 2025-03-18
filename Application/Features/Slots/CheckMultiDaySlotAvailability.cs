using Application.Interfaces;

namespace Application.Features.Slots
{
    public class CheckMultiDaySlotAvailabilityQuery : IRequest<List<int>>
    {
        public int CourtId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public List<string> DaysOfWeek { get; set; } = new List<string>();
    }
    public class CheckMultiDaySlotAvailabilityQueryHandler : IRequestHandler<CheckMultiDaySlotAvailabilityQuery, List<int>>
    {
        private readonly ICheckMultiDaySlotAvailabilityQuery _query;

        public CheckMultiDaySlotAvailabilityQueryHandler(ICheckMultiDaySlotAvailabilityQuery query)
        {
            _query = query;
        }

        public async Task<List<int>> Handle(CheckMultiDaySlotAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var result = await _query.Execute(request);
            return result;
        }
    }
}
