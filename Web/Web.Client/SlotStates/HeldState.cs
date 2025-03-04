
namespace Web.Client.SlotStates
{
    public class HeldState : ISlotState
    {
        public string StatusText => "Đang chọn";

        public string StatusColor => "#ffffff";

        public Task HandleClick(SlotComponent slot)
        {
            return Task.CompletedTask;
        }
    }
}
