namespace Web.Client.SlotStates
{
    public class HeldState : ISlotState
    {
        public string StatusText => "Khóa";

        public string StatusColor => "#bfbfbf";

        public Task HandleClick(SlotComponent slot)
        {
            return Task.CompletedTask;
        }
    }
}
