using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;

namespace Web.Services
{
    public class SlotNotification : ISlotNotification
    {
        private readonly IHubContext<SlotHub> _hubContext;

        public SlotNotification(IHubContext<SlotHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifySlotHeldAsync(object payload, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync("SlotHeld", payload, cancellationToken);
        }

        public async Task NotifySlotReleasedAsync(object payload, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync("SlotReleased", payload, cancellationToken);
        }
    }
}
