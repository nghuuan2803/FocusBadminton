namespace Web.Client.StatePattern
{
    public class LockedState : ISlotState
    {
        public string DisplayName => "Khóa";
        public string CssClass => "slot-locked";
        public bool CanSelect => false;

        public void HandleSelection(SlotContext context)
        {
            // Không cho chọn vì bị người khác giữ
        }
    }
}
