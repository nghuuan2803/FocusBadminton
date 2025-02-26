namespace Web.Client.StatePattern
{
    public class BookedState : ISlotState
    {
        public string DisplayName => "Đã đặt";
        public string CssClass => "slot-booked";
        public bool CanSelect => false;

        public void HandleSelection(SlotContext context)
        {
            // Không cho chọn vì đã được đặt
        }
    }
}
