using Moq;
using OrderManagement.Models;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Tests
{
    public class ProductTests
    {
        private readonly Mock<IProductRepository> _repositoryMock;
        private readonly Mock<IOrderRepository> _repositoryOrderMock;
        private readonly ProductService _service;

        public ProductTests()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _repositoryOrderMock = new Mock<IOrderRepository>();
            _service = new ProductService(_repositoryMock.Object, _repositoryOrderMock.Object);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnProduct_WhenFound()
        {
            var product = new Product { Id = 1, Name = "Produto A", Price = 10.0m, StockQuantity = 5 };
            _repositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(product);

            var result = await _service.GetProductById(1);

            Assert.Equal("Produto A", result.Name);
        }

        [Fact]
        public async Task GetProductById_ShouldThrowException_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.GetById(1)).ReturnsAsync((Product)null);

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetProductById(1));
            Assert.Contains("Pedido com ID 1 não encontrado", ex.Message);
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnAllProducts()
        {
            var products = new List<Product>
        {
            new Product { Id = 1, Name = "Produto A" },
            new Product { Id = 2, Name = "Produto B" }
        };
            _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(products);

            var result = await _service.GetAllProducts();

            Assert.Equal(2, ((List<Product>)result).Count);
        }

        [Fact]
        public async Task GetAllProducts_ShouldThrowException_OnError()
        {
            _repositoryMock.Setup(r => r.GetAll()).ThrowsAsync(new Exception("Erro no banco"));

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetAllProducts());
            Assert.Contains("Erro no banco", ex.Message);
        }

        [Fact]
        public async Task GetProductsByName_ShouldReturnFiltered()
        {
            var products = new List<Product> { new Product { Id = 1, Name = "Produto C" } };
            _repositoryMock.Setup(r => r.GetByName("Produto C")).ReturnsAsync(products);

            var result = await _service.GetProductsByName("Produto C");

            Assert.Single(result);
        }

        [Fact]
        public async Task CreateProduct_ShouldThrowException_WhenNameInvalid()
        {
            var product = new Product { Name = "", Price = 10, StockQuantity = 10 };

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateProduct(product));
            Assert.Contains("nome do produto é obrigatório", ex.Message);
        }

        [Fact]
        public async Task CreateProduct_ShouldThrowException_WhenPriceInvalid()
        {
            var product = new Product { Name = "Produto", Price = -5, StockQuantity = 10 };

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateProduct(product));
            Assert.Contains("preço do produto é inválido", ex.Message);
        }

        [Fact]
        public async Task CreateProduct_ShouldSucceed_WhenValid()
        {
            var product = new Product { Name = "Produto", Price = 10, StockQuantity = 5 };

            await _service.CreateProduct(product);

            _repositoryMock.Verify(r => r.Create(product), Times.Once);
        }

        [Fact]
        public async Task UpdateProduct_ShouldThrowException_WhenProductNotFound()
        {
            var product = new Product { Id = 1, Name = "Novo", Price = 10, StockQuantity = 1 };
            _repositoryMock.Setup(r => r.GetById(1)).ReturnsAsync((Product)null);

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.UpdateProduct(product));
            Assert.Contains("Produto com ID 1 não encontrado", ex.Message);
        }

        [Fact]
        public async Task UpdateProduct_ShouldSucceed_WhenValid()
        {
            var product = new Product { Id = 1, Name = "Produto Atualizado", Price = 20, StockQuantity = 10 };
            _repositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(product);
            _repositoryMock.Setup(r => r.Update(product)).ReturnsAsync(product);

            var result = await _service.UpdateProduct(product);

            Assert.Equal("Produto Atualizado", result.Name);
        }

        [Fact]
        public async Task DeleteProduct_ShouldThrowException_WhenProductNotFound()
        {
            _repositoryMock.Setup(r => r.GetById(1)).ReturnsAsync((Product)null);

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.DeleteProduct(1));
            Assert.Contains("Produto com ID 1 não encontrado", ex.Message);
        }

        [Fact]
        public async Task DeleteProduct_ShouldSucceed_WhenProductExists()
        {
            var product = new Product { Id = 1, Name = "Produto", Price = 10, StockQuantity = 2 };
            _repositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(product);

            await _service.DeleteProduct(1);

            _repositoryMock.Verify(r => r.Delete(1), Times.Once);
        }
    }
}
