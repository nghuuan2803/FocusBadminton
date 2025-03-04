namespace Web.Client.SlotStates
{
    public interface ISlot
    {
        void OnHeld(object payload);
        void OnReleased(object payload);
        void OnBookingCreated(object payload);
        void OnBookingCanceled(object payload);
        void OnBookingApproved(object payload);
        void OnBookingRejected(object payload);
        void OnBookingPaused(object payload);
        void OnBookingResumed(object payload);
        void OnBookingCompleted(object payload);
        void OnSlotTimeOut();
    }
}
