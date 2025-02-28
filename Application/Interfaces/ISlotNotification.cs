namespace Application.Interfaces
{
    public interface ISlotNotification
    {
        Task NotifySlotHeldAsync(object payload, CancellationToken cancellationToken);
        Task NotifySlotReleasedAsync(object payload, CancellationToken cancellationToken);
        Task NotifyBookingCreatedAsync(int bookingId, CancellationToken cancellationToken);
    }
}
