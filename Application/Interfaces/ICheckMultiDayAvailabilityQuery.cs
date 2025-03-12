using Application.Features.Slots;
using Shared.Schedules;

namespace Application.Interfaces
{
    public interface ICheckMultiDaySlotAvailabilityQuery
    {
        Task<List<int>> Execute(CheckMultiDaySlotAvailabilityQuery request, CancellationToken cancellation = default);
    }
}
