namespace Web.Client.SlotStates
{
    public class PausedState : ISlotState
    {
        public string StatusText => "Tạm ngưng";
        public string StatusColor => "#c41d7f";
        public async Task HandleClick(SlotComponent slot)
        {
            // xem chi tiết booking
            await slot.ViewBookingDetailsAsync();
        }
    }
}
