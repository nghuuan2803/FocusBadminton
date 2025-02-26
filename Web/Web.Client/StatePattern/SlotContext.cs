namespace Web.Client.StatePattern
{
    public class SlotContext : IBookingComponent
    {
        private ISlotState _state;
        public int CourtId { get; }
        public int TimeSlotId { get; }
        public DateTime Date { get; }
        public string StartTime { get; }
        public string EndTime { get; }
        public int? HoldId { get; private set; } // Lưu HoldId
        private readonly IBookingMediator _mediator;

        public SlotContext(int courtId, int timeSlotId, DateTime date, string startTime, string endTime, IBookingMediator mediator)
        {
            CourtId = courtId;
            TimeSlotId = timeSlotId;
            Date = date;
            StartTime = startTime;
            EndTime = endTime;
            _mediator = mediator;
            _state = new AvailableState();
        }

        public void SetState(ISlotState state) => _state = state;
        public ISlotState GetState() => _state;

        public void SelectSlot()
        {
            _state.HandleSelection(this);
        }

        public void RequestHoldSlot()
        {
            _mediator.Notify(this, "SlotSelected", this);
        }

        public void ReceiveNotification(string eventName, object data)
        {
            if (eventName == "ClearSlots" && _state is SelectingState)
            {
                SetState(new AvailableState());
            }
        }
    }

    // Cập nhật AvailableState để dùng Mediator
    public class AvailableState : ISlotState
    {
        public string DisplayName => "Trống";
        public string CssClass => "slot-available";
        public bool CanSelect => true;

        public void HandleSelection(SlotContext context)
        {
            context.SetState(new SelectingState());
            context.RequestHoldSlot();
        }
    }
}
