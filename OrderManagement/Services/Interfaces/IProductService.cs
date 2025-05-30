using OrderManagement.Models;

namespace OrderManagement.Services.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetProductById(int id);
        Task<IEnumerable<Product>> GetProductsByName(string name);
        Task<IEnumerable<Product>> GetAllProducts();
        Task CreateProduct(Product product);
        Task<Product> UpdateProduct(Product product);
        Task DeleteProduct(int id);
    }
}
