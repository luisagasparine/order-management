using Dapper;
using OrderManagement.Models;
using OrderManagement.Repositories.Interfaces;
using System.Data;

namespace OrderManagement.Repositories
{
    public class NotificationStatusRepository : INotificationStatusRepository
    {
        private readonly IDbConnection _dbConnection;

        public NotificationStatusRepository(IDbConnection dbConnection) { 
            _dbConnection = dbConnection;
        }

        public async Task CreateNotification(NotificationsStatusOrder notificationsStatus)
        {
            var query = "INSERT INTO NotificationsStatusOrder (OrderId, StatusFrom, StatusTo, DateChanged) VALUES (@OrderId, @StatusFrom, @StatusTo, @DateChanged)";
            _dbConnection.Execute(query, notificationsStatus);
        }
    }
}
