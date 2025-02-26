namespace Web.Client.StatePattern
{
    public class UnavailableState : ISlotState
    {
        public string DisplayName => "Không được chọn";
        public string CssClass => "slot-unavailable";
        public bool CanSelect => false;

        public void HandleSelection(SlotContext context)
        {
            // Không cho chọn vì quá giờ hiện tại
        }
    }
}
