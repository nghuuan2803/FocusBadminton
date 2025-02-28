namespace Web.Client.SlotStates
{
    public class PausedState : ISlotState
    {
        public string StatusText => "Tạm dừng";
        public string StatusColor => "#f8d7da"; // Màu đỏ nhạt
        public async Task HandleClick(SlotComponent slot)
        {
            // xem chi tiết booking
            await slot.ViewBookingDetailsAsync();
        }
    }
}
