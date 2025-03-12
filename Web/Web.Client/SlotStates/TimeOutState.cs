namespace Web.Client.SlotStates
{
    public class TimeOutState : ISlotState
    {
        public string StatusText => "-";
        public string StatusColor => "#8c8c8c"; // Màu đỏ nhạt
        public async Task HandleClick(SlotComponent slot)
        {
            // không làm gì
            await Task.CompletedTask;
        }
    }
}
