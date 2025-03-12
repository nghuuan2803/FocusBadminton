namespace Web.Client.SlotStates
{
    public class HeldState : ISlotState
    {
        public string StatusText => "Tạm khóa";

        public string StatusColor => "#bfbfbf";

        public Task HandleClick(SlotComponent slot)
        {
            return Task.CompletedTask;
        }
    }
}
