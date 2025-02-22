using Application.Interfaces.DapperQueries;
using Shared.Schedules;

namespace Application.Features.Schedules
{
    public class GetCourtSchedulesQuery : IRequest<IEnumerable<ScheduleDTO>>
    {
        public DateTime Date { get; set; }
        public int FacilityId { get; set; }
    }

    public class GetCourtSchedulesQueryHandler : IRequestHandler<GetCourtSchedulesQuery, IEnumerable<ScheduleDTO>>
    {
        private readonly IScheduleQueries _queries;
        public GetCourtSchedulesQueryHandler(IScheduleQueries queries)
        {
            _queries = queries;
        }
        public async Task<IEnumerable<ScheduleDTO>> Handle(GetCourtSchedulesQuery request, CancellationToken cancellationToken)
        {
            return await _queries.GetAllCourtSchedulesAsync(request.Date, request.FacilityId);
        }
    }

}
