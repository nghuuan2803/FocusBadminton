namespace Web.Client.SlotStates
{
    public class AvailableState : ISlotState
    {
        public string StatusText => "Trống";
        public string StatusColor => "#fff";
        //public string StatusColor => "#b5f5ec"; // Màu xanh nhạt


        public async Task HandleClick(SlotComponent slot)
        {
            // Logic giữ chỗ: Gọi API hoặc mở modal
            await slot.HoldSlotAsync();
            slot.TransitionTo(new HoldingState());
        }
    }
}
