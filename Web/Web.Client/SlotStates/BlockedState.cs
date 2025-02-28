namespace Web.Client.SlotStates
{
    public class BlockedState : ISlotState
    {
        public string StatusText => "Đã chặn";
        public string StatusColor => "#f8d7da"; // Màu đỏ nhạt
        public async Task HandleClick(SlotComponent slot)
        {
            // không làm gì
            await Task.CompletedTask;
        }
    }
}
