namespace Web.Client.StatePattern
{
    public class SelectingState : ISlotState
    {
        public string DisplayName => "Đang chọn";
        public string CssClass => "slot-selecting";
        public bool CanSelect => false;

        public void HandleSelection(SlotContext context)
        {
            // Không làm gì vì slot đang được giữ bởi người dùng hiện tại
        }
    }
}
