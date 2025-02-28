namespace Web.Client.SlotStates
{
    public class TimeOutState : ISlotState
    {
        public string StatusText => "Hết thời gian";
        public string StatusColor => "#f8d7da"; // Màu đỏ nhạt
        public async Task HandleClick(SlotComponent slot)
        {
            // không làm gì
            await Task.CompletedTask;
        }
    }
}
