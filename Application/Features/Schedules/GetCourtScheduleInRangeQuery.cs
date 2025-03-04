using Application.Interfaces.DapperQueries;
using Shared.Schedules;

namespace Application.Features.Schedules
{
    public class GetCourtScheduleInRangeQuery : IRequest<IEnumerable<CourtScheduleDTO>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CourtId { get; set; }
    }
    public class GetCourtScheduleInRangeQueryHandler : IRequestHandler<GetCourtScheduleInRangeQuery, IEnumerable<CourtScheduleDTO>>
    {
        private readonly IScheduleQueries _queries;

        public GetCourtScheduleInRangeQueryHandler(IScheduleQueries queries)
        {
            _queries = queries;
        }

        public async Task<IEnumerable<CourtScheduleDTO>> Handle(GetCourtScheduleInRangeQuery request, CancellationToken cancellationToken)
        {
            var data = await _queries.GetCourtSchedulesAsync(request.StartDate, request.EndDate, request.CourtId);
            return data;
        }
    }
}
