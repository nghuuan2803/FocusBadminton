namespace Web.Client.SlotStates
{
    public class AvailableState : ISlotState
    {
        public string StatusText => "Trống";
        public string StatusColor => "#fff";
        public async Task HandleClick(SlotComponent slot)
        {
            // Logic giữ chỗ: Gọi API hoặc mở modal
            await slot.HoldSlotAsync();
        }
    }
}
