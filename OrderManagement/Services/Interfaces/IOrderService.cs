using OrderManagement.Models;
using OrderManagement.ViewModels;

namespace OrderManagement.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> GetOrderById(int id);
        Task<IEnumerable<Order>> GetOrdersByCustomerOrStatus(int? idCustomer, string status);
        Task<Order> GetOrdersDetails(int idOrder);
        Task<Order> CreateOrder(NewOrderViewModel newOrder);
        Task UpdateStatusOrder(UpdateStatusOrderViewModel order);
    }
}
