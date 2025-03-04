﻿namespace Web.Client.SlotStates
{
    public class BlockedState : ISlotState
    {
        public string StatusText => "Chặn";
        public string StatusColor => "#000000";
        public async Task HandleClick(SlotComponent slot)
        {
            // không làm gì
            await slot.UnblockSlotAsync();
        }
    }
}
