namespace Web.Client.SlotStates
{
    public interface ISlotState
    {
        string StatusText { get; }           // Trả về text hiển thị (ví dụ: "Trống", "Chờ duyệt")
        string StatusColor { get; }          // Trả về màu sắc của ô
        Task HandleClick(SlotComponent slot); // Hành động khi click vào ô
    }
}
