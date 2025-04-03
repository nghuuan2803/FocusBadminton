namespace Web.Client.SlotStates
{
    public class CreatingState : ISlotState
    {
        public string StatusText => "Đang tạo";
        public string StatusColor => "#FFF1D5";
        public async Task HandleClick(SlotComponent slot)
        {
           await slot.ViewBookingDetailsAsync();
        }
    }
}
