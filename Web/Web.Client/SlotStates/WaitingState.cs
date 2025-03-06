namespace Web.Client.SlotStates
{
    public class WaitingState : ISlotState
    {
        public string StatusText => "Trống";
        public string StatusColor => "#fff";
        //public string StatusColor => "#b5f5ec"; // Màu xanh nhạt


        public Task HandleClick(SlotComponent slot)
        {
            return Task.CompletedTask;
        }
    }
}
