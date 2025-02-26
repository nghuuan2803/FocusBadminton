namespace Web.Client.MediatorPattern
{
    public class BookingMediator : IBookingMediator
    {
        private readonly List<IBookingComponent> _components = new();

        public void RegisterComponent(IBookingComponent component)
        {
            if (!_components.Contains(component))
            {
                _components.Add(component);
            }
        }

        public void Notify(object sender, string eventName, object data = null)
        {
            foreach (var component in _components)
            {
                if (component != sender) // Tránh gửi lại cho chính nó
                {
                    component.ReceiveNotification(eventName, data);
                }
            }
        }
    }
}
