namespace Web.Client.SlotStates
{
    public interface ISlotState
    {
        string StatusText { get; }
        string StatusColor { get; }
        Task HandleClick(SlotComponent slot);
    }
}
