namespace Web.Client.SlotStates
{
    public class CompletedState : ISlotState
    {
        public string StatusText => "Kết thúc";
        public string StatusColor => "#389e0d";
        public async Task HandleClick(SlotComponent slot)
        {
            // Hiện modal thông tin đặt sân
            await slot.ViewBookingDetailsAsync();
        }
    }
}
