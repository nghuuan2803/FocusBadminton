namespace Web.Client.SlotStates
{
    public class PendingState : ISlotState
    {
        public string StatusText => "Chưa duyệt";
        public string StatusColor => "#fff3cd"; // Màu vàng nhạt
        public async Task HandleClick(SlotComponent slot)
        {
            // Logic xem đơn đặt sân: Mở modal hoặc chuyển hướng
            await slot.ViewBookingDetailsAsync();
        }
    }
}
