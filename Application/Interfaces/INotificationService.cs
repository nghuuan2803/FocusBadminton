using Domain.Entities;

namespace Sh.Interfaces
{
    public interface INotificationService
    {
        Task SendToAll(Notification notification);
        Task SendToUser(Notification notification);
    }
}
