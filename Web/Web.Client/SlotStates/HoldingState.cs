namespace Web.Client.SlotStates
{
    public class HoldingState : ISlotState
    {
        public string StatusText => "Đang giữ";
        public string StatusColor => "#f8d7da"; // Màu đỏ nhạt
        public async Task HandleClick(SlotComponent slot)
        {
            // Logic hủy giữ chỗ: Gọi API
            await slot.CancelHoldAsync();
        }
    }
}
