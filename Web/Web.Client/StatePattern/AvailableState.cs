namespace Web.Client.StatePattern
{
    public class AvailableState : ISlotState
    {
        public string DisplayName => "Trống";
        public string CssClass => "slot-available";
        public bool CanSelect => true;

        public void HandleSelection(SlotContext context)
        {
            // Chuyển sang trạng thái "Đang chọn" và gọi API giữ slot
            context.SetState(new SelectingState());
            context.RequestHoldSlot();
        }
    }
}
