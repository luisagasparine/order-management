using OrderManagement.Models;

namespace OrderManagement.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> GetById(int id);
        Task<IEnumerable<Customer>> GetByNameOrEmail(string name, string email);
        Task<Customer> GetByEmail(string email);
        Task<IEnumerable<Customer>> GetAll();
        Task Create(Customer customer);
        Task<Customer> Update(Customer customer);
        Task Delete(int id);
    }
}
