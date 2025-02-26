namespace Web.Client.MediatorPattern
{
    public interface IBookingMediator
    {
        void RegisterComponent(IBookingComponent component);
        void Notify(object sender, string eventName, object data = null);
    }
}
