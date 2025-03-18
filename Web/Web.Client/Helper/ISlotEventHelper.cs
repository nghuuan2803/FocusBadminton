
namespace Web.Client.Helper
{
    public interface ISlotEventHelper
    {
        event Action<object>? OnBookingApproved;
        event Action<object>? OnBookingCanceled;
        event Action<object>? OnBookingCompleted;
        event Action<object>? OnBookingCreated;
        event Action<object>? OnBookingPaused;
        event Action<object>? OnBookingRejected;
        event Action<object>? OnBookingResumed;
        event Action<object>? OnSlotHeld;
        event Action<object>? OnSlotReleased;

        ValueTask DisposeAsync();
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
    }
}