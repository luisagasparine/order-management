using OrderManagement.Models;

namespace OrderManagement.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetById(int id);
        Task<IEnumerable<Order>> GetByCustomerOrStatus(int? idCustomer, string status);
        Task<int> GetOrderCountByCustomerId(int customerId);
        Task<int> GetOrderItemCountByProductId(int productId);
        Task<Order> GetDetails(int id);
        Task<Order> Create(Order order);
        Task<Order> Update(Order order);
    }
}
