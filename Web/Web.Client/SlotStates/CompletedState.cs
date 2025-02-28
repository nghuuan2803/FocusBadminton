namespace Web.Client.SlotStates
{
    public class CompletedState : ISlotState
    {
        public string StatusText => "Kết thúc";
        public string StatusColor => "#28a745"; // Màu xanh lá cây
        public async Task HandleClick(SlotComponent slot)
        {
            // Hiện modal thông tin đặt sân
            await slot.ViewBookingDetailsAsync();
        }
    }
}
