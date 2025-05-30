using OrderManagement.Models;
using OrderManagement.Repositories;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;

        public ProductService(IProductRepository productRepository, IOrderRepository orderRepository) {
            _productRepository = productRepository;
            _orderRepository = orderRepository;
        }

        public async Task<Product> GetProductById(int id)
        {
            try
            {
                var product = await _productRepository.GetById(id);

                if (product == null)
                    throw new ArgumentException($"Pedido com ID {id} não encontrado.");
                
                return product;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            try
            {
                return await _productRepository.GetByName(name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            try
            {
                return await _productRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateProduct(Product product)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(product.Name))
                    throw new ArgumentException("O nome do produto é obrigatório.");

                if (product.Price < 0)
                    throw new ArgumentException("O preço do produto é inválido.");

                if (product.StockQuantity < 0)
                    throw new ArgumentException("O estoque do produto é inválido.");

                await _productRepository.Create(product);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(product.Name))
                    throw new ArgumentException("O nome do produto é obrigatório.");

                if (product.Price < 0)
                    throw new ArgumentException("O preço do produto é inválido.");

                if (product.StockQuantity < 0)
                    throw new ArgumentException("O estoque do produto é inválido.");

                var existingProduct = await _productRepository.GetById(product.Id);
                if (existingProduct == null)
                    throw new ArgumentException($"Produto com ID {product.Id} não encontrado.");

                return await _productRepository.Update(product);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
         
        }

        public async Task DeleteProduct(int id)
        {
            try
            {
                var existingProduct = await _productRepository.GetById(id);
                if (existingProduct == null)
                    throw new ArgumentException($"Produto com ID {id} não encontrado.");

                var orderByProduct = await _orderRepository.GetOrderItemCountByProductId(existingProduct.Id);
                if(orderByProduct > 0)
                    throw new ArgumentException($"Produto com ID {id} não pode ser excluído, pois possui pedidos vinculados.");

                await _productRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
