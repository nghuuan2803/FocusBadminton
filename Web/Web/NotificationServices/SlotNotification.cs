using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;

namespace Web.NotificationServices
{
    public class SlotNotification : ISlotNotification
    {
        private readonly IHubContext<SlotHub> _hubContext;
        private readonly ILogger<SlotNotification> _logger;

        public SlotNotification(IHubContext<SlotHub> hubContext, ILogger<SlotNotification> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task NotifySlotHeldAsync(object payload, CancellationToken cancellationToken)
        {
            if (payload == null)
            {
                _logger.LogWarning("Payload is null in NotifySlotHeldAsync, skipping.");
                return;
            }

            try
            {
                _logger.LogInformation("Sending SlotHeld with payload: {@Payload}", payload);
                await _hubContext.Clients.All.SendAsync("SlotHeld", payload, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SlotHeld notification: {Message}", ex.Message);
                throw;
            }
        }

        public async Task NotifySlotReleasedAsync(object payload, CancellationToken cancellationToken)
        {
            if (payload == null)
            {
                _logger.LogWarning("Payload is null in NotifySlotReleasedAsync, skipping.");
                return;
            }

            try
            {
                _logger.LogInformation("Sending SlotReleased with payload: {@Payload}", payload);
                await _hubContext.Clients.All.SendAsync("SlotReleased", payload, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SlotReleased notification: {Message}", ex.Message);
                throw;
            }
        }

        public async Task NotifyBookingCreatedAsync(int bookingId, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Sending BookingCreated with BookingId: {BookingId}", bookingId);
                await _hubContext.Clients.All.SendAsync("BookingCreated", bookingId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending BookingCreated notification: {Message}", ex.Message);
                throw;
            }
        }
    }
}
