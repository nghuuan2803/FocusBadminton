namespace Web.Client.MediatorPattern
{
    public interface IBookingComponent
    {
        void ReceiveNotification(string eventName, object data);
    }
}
