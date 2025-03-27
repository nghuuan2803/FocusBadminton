using Application.Interfaces;
using Shared.Statistic;

namespace Application.Features.Statictis
{
    public class TimeSlotStatictisQuery : IRequest<TimeSlotStatisticDTO>
    {
        public int Year { get; set; }
        public int? Month { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
    }
    public class TimeSlotStatictisQueryHandler : IRequestHandler<TimeSlotStatictisQuery, TimeSlotStatisticDTO>
    {
        ITimeSlotStatisticQueries _timeSlotQueries;

        public TimeSlotStatictisQueryHandler(ITimeSlotStatisticQueries timeSlotQueries)
        {
            _timeSlotQueries = timeSlotQueries;
        }

        public async Task<TimeSlotStatisticDTO> Handle(TimeSlotStatictisQuery request, CancellationToken cancellationToken)
        {
           return await _timeSlotQueries.Excute(request, cancellationToken);
        }
    }
}
