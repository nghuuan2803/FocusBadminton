namespace Web.Client.SlotStates
{
    public class HoldingState : ISlotState
    {
        public string StatusText => "Đang giữ";
        public string StatusColor => "#13c2c2";
        public async Task HandleClick(SlotComponent slot)
        {
            // Logic hủy giữ chỗ: Gọi API
            await slot.CancelHoldAsync();
        }
    }
}
