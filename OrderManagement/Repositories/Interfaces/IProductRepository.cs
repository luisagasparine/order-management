using OrderManagement.Models;

namespace OrderManagement.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetById(int id);
        Task<IEnumerable<Product>> GetByName(string name);
        Task<IEnumerable<Product>> GetAll();
        Task Create(Product product);
        Task<Product> Update(Product product);
        Task Delete(int id);
    }
}
