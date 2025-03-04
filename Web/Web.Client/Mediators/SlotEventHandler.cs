using Microsoft.Extensions.Logging;
using Web.Client.Pages;

namespace Web.Client.Mediators
{
    public class SlotEventHandler : IScheduleMediator, IAsyncDisposable
    {
        private SlotEventListener _slotEventListener;
        private readonly Dictionary<string, SlotComponent> _slot = new();

        public SlotEventHandler(IConfiguration configuration)
        {
            var config = configuration.GetSection("ApiSettings");
            string apiUrl = config["BaseAddress"];
            _slotEventListener = new SlotEventListener(apiUrl);
        }
        public async Task StartAsync()
        {
            await _slotEventListener.StartAsync();
        }
        public void AddSlot(SlotComponent slot)
        {
            var key = $"{slot.CourtId}-{slot.TimeSlotId}-{slot.Date.ToString()}";
            if (!_slot.ContainsKey(key))
            {
                _slot.Add(key, slot);
            }
        }

        public void RemoveSlot(SlotComponent slot)
        {
            var key = $"{slot.CourtId}-{slot.TimeSlotId}-{slot.Date.ToString()}";
            if (_slot.ContainsKey(key))
            {
                _slot.Remove(key);
            }
        }
        public void SlotHeldNotify(object payload)
        {
            throw new NotImplementedException();
        }

        public void SlotReleasedNotify(object payload)
        {
            throw new NotImplementedException();
        }
        public void BookingApprovedNotify(object payload)
        {

        }

        public void BookingCaneledNotify(object payload)
        {
            throw new NotImplementedException();
        }

        public void BookingCompletedNotify(object payload)
        {
            throw new NotImplementedException();
        }

        public void BookingCreatedNotify(object payload)
        {
            throw new NotImplementedException();
        }

        public void BookingPausedNotify(object payload)
        {
            throw new NotImplementedException();
        }

        public void BookingRejectedNotify(object payload)
        {
            throw new NotImplementedException();
        }

        public void BookingResumedNotify(object payload)
        {
            throw new NotImplementedException();
        }

        public void TimeOutNotify()
        {
            // Transition all slots to the TimeOutState
            foreach (var slot in _slot.Values)
            {
                if (slot.StartAt < DateTimeOffset.Now)
                    slot.TransitionTo(new TimeOutState());
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _slotEventListener.DisposeAsync();
        }
    }
}