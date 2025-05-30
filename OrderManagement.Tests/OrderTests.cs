using Moq;
using OrderManagement.Models;
using OrderManagement.ViewModels;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Services.Interfaces;
using OrderManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagement.Tests
{
    public class OrderTests
    {

        private readonly Mock<IOrderRepository> _orderRepoMock = new();
        private readonly Mock<ICustomerService> _customerServiceMock = new();
        private readonly Mock<IProductService> _productServiceMock = new();
        private readonly Mock<INotificationStatusRepository> _notificationRepoMock = new();
        private readonly OrderService _service;

        public OrderTests()
        {
            _service = new OrderService(
                _orderRepoMock.Object,
                _customerServiceMock.Object,
                _productServiceMock.Object,
                _notificationRepoMock.Object
            );
        }

        [Fact]
        public async Task GetOrderById_ShouldReturnOrder_WhenExists()
        {
            var order = new Order { Id = 1 };
            _orderRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(order);

            var result = await _service.GetOrderById(1);

            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetOrderById_ShouldThrow_WhenNotFound()
        {
            _orderRepoMock.Setup(r => r.GetById(1)).ReturnsAsync((Order)null);

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetOrderById(1));
            Assert.Contains("Pedido com ID 1 não encontrado", ex.Message);
        }

        [Fact]
        public async Task GetOrdersByCustomerOrStatus_ShouldReturnList()
        {
            var orders = new List<Order> { new Order { Id = 1 } };
            _orderRepoMock.Setup(r => r.GetByCustomerOrStatus(null, "Novo")).ReturnsAsync(orders);

            var result = await _service.GetOrdersByCustomerOrStatus(0, "Novo");

            Assert.Single(result);
        }

        [Fact]
        public async Task GetOrdersDetails_ShouldReturnDetails()
        {
            var order = new Order { Id = 1 };
            _orderRepoMock.Setup(r => r.GetDetails(1)).ReturnsAsync(order);

            var result = await _service.GetOrdersDetails(1);

            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateOrder_ShouldThrow_WhenCustomerNotFound()
        {
            _customerServiceMock.Setup(c => c.GetCustomerById(1)).ReturnsAsync((Customer)null);

            var newOrder = new NewOrderViewModel { CustomerId = 1 };

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateOrder(newOrder));
            Assert.Contains("Cliente não encontrado", ex.Message);
        }

        [Fact]
        public async Task CreateOrder_ShouldThrow_WhenProductNotFound()
        {
            _customerServiceMock.Setup(c => c.GetCustomerById(1)).ReturnsAsync(new Customer());
            _productServiceMock.Setup(p => p.GetProductById(1)).ReturnsAsync((Product)null);

            var newOrder = new NewOrderViewModel
            {
                CustomerId = 1,
                Items = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 2 } }
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateOrder(newOrder));
            Assert.Contains("Produto com ID 1 não encontrado", ex.Message);
        }

        [Fact]
        public async Task CreateOrder_ShouldThrow_WhenInsufficientStock()
        {
            _customerServiceMock.Setup(c => c.GetCustomerById(1)).ReturnsAsync(new Customer());
            _productServiceMock.Setup(p => p.GetProductById(1))
                .ReturnsAsync(new Product { Id = 1, StockQuantity = 1, Name = "Produto A" });

            var newOrder = new NewOrderViewModel
            {
                CustomerId = 1,
                Items = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 2 } }
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateOrder(newOrder));
            Assert.Contains("Estoque insuficiente", ex.Message);
        }

        [Fact]
        public async Task CreateOrder_ShouldSucceed_WhenValid()
        {
            var product = new Product { Id = 1, Name = "Produto A", StockQuantity = 10, Price = 100 };
            var customer = new Customer { Id = 1 };

            _customerServiceMock.Setup(c => c.GetCustomerById(1)).ReturnsAsync(customer);
            _productServiceMock.Setup(p => p.GetProductById(1)).ReturnsAsync(product);
            _productServiceMock.Setup(p => p.UpdateProduct(It.IsAny<Product>())).ReturnsAsync(product);
            _orderRepoMock.Setup(r => r.Create(It.IsAny<Order>())).ReturnsAsync(new Order { Id = 10 });

            var newOrder = new NewOrderViewModel
            {
                CustomerId = 1,
                Items = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 1 } }
            };

            var result = await _service.CreateOrder(newOrder);

            Assert.Equal(10, result.Id);
        }

        [Fact]
        public async Task UpdateStatusOrder_ShouldThrow_WhenInvalidStatus()
        {
            var update = new UpdateStatusOrderViewModel
            {
                Id = 1,
                Status = "Enviado"
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.UpdateStatusOrder(update));
            Assert.Contains("status do pedido é inválido", ex.Message);
        }

        [Fact]
        public async Task UpdateStatusOrder_ShouldThrow_WhenOrderNotFound()
        {
            _orderRepoMock.Setup(r => r.GetById(1)).ReturnsAsync((Order)null);

            var update = new UpdateStatusOrderViewModel
            {
                Id = 1,
                Status = StatusOrderType.Finalizado.ToString()
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.UpdateStatusOrder(update));
            Assert.Contains("Pedido com ID 1 não encontrado", ex.Message);
        }

        [Fact]
        public async Task UpdateStatusOrder_ShouldUpdateAndNotify_WhenStatusChanged()
        {
            var order = new Order { Id = 1, Status = "Novo" };

            _orderRepoMock.Setup(r => r.GetById(1)).ReturnsAsync(order);
            _orderRepoMock.Setup(r => r.Update(It.IsAny<Order>())).ReturnsAsync(order);

            var update = new UpdateStatusOrderViewModel
            {
                Id = 1,
                Status = StatusOrderType.Processando.ToString()
            };

            await _service.UpdateStatusOrder(update);

            _orderRepoMock.Verify(r => r.Update(It.IsAny<Order>()), Times.Once);
            _notificationRepoMock.Verify(n => n.CreateNotification(It.IsAny<NotificationsStatusOrder>()), Times.Once);
        }
    }
}
