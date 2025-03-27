using Application.Features.Statictis;
using Shared.Statistic;

namespace Application.Interfaces
{
    public interface ITimeSlotStatisticQueries
    {
        Task<TimeSlotStatisticDTO> Excute(TimeSlotStatictisQuery request, CancellationToken cancellationToken);
    }
}
