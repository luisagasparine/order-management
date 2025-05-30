using OrderManagement.Models;

namespace OrderManagement.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerById(int id);
        Task<IEnumerable<Customer>> GetCustomersByNameOrEmail(string name, string email);
        Task<IEnumerable<Customer>> GetAllCustomers();
        Task CreateCustomer(Customer customer);
        Task<Customer> UpdateCustomer(Customer customer);
        Task DeleteCustomer(int id);
    }
}
