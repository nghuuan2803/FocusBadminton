using Application.Interfaces.DapperQueries;
using Shared.Schedules;

namespace Application.Features.Schedules
{
    public class GetCourtScheduleQuery : IRequest<IEnumerable<ScheduleDTO>>
    {
        public DateTime Date { get; set; }
        public int CourtId { get; set; }
    }

    public class GetCourtScheduleQueryHandler : IRequestHandler<GetCourtScheduleQuery, IEnumerable<ScheduleDTO>>
    {
        private readonly IScheduleQueries _queries;
        public GetCourtScheduleQueryHandler(IScheduleQueries queries)
        {
            _queries = queries;
        }
        public async Task<IEnumerable<ScheduleDTO>> Handle(GetCourtScheduleQuery request, CancellationToken cancellation)
        {
            return await _queries.GetCourtSchedulesAsync(request.Date, request.CourtId);
        }
    }
}
