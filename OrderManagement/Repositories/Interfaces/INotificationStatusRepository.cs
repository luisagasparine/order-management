using OrderManagement.Models;

namespace OrderManagement.Repositories.Interfaces
{
    public interface INotificationStatusRepository
    {
        Task CreateNotification(NotificationsStatusOrder notificationsStatus);
    }
}
