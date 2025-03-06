namespace Web.Client.SlotStates
{
    public class BookedState : ISlotState
    {
        public string StatusText => "Đã đặt";
        public string StatusColor => "#d4380d";
        public async Task HandleClick(SlotComponent slot)
        {
            // Có thể không làm gì hoặc xem thông tin (tùy yêu cầu)
            await slot.ViewBookingDetailsAsync();
        }
    }
}
