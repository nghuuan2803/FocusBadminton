using Domain.Entities;

namespace Application.Interfaces
{
    public interface INotificationService
    {
        Task SendToAll(Notification notification);
        Task SendToUser(Notification notification);
    }
}
