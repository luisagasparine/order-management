using Moq;
using OrderManagement.Models;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Services;

namespace OrderManagement.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly Mock<IOrderRepository> _repositoryOrderMock;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _repositoryMock = new Mock<ICustomerRepository>();
            _repositoryOrderMock = new Mock<IOrderRepository>();
            _service = new CustomerService(_repositoryMock.Object, _repositoryOrderMock.Object);
        }

        [Fact]
        public async Task GetCustomerById_ShouldReturnCustomer_WhenFound()
        {
            var customer = new Customer { Id = 1, Name = "Test", Email = "test@example.com" };
            _repositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(customer);

            var result = await _service.GetCustomerById(1);

            Assert.Equal("Test", result.Name);
        }

        [Fact]
        public async Task GetCustomerById_ShouldThrowException_WhenNotFound()
        {
            _repositoryMock.Setup(r => r.GetById(1)).ReturnsAsync((Customer)null);

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetCustomerById(1));
            Assert.Contains("Cliente com ID 1 não encontrado", ex.Message);
        }

        [Fact]
        public async Task CreateCustomer_ShouldThrowException_WhenEmailIsInvalid()
        {
            var customer = new Customer { Name = "Invalid", Email = "invalidemail" };

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateCustomer(customer));
            Assert.Contains("email fornecido é inválido", ex.Message);
        }

        [Fact]
        public async Task CreateCustomer_ShouldThrowException_WhenEmailAlreadyExists()
        {
            var customer = new Customer { Name = "Existing", Email = "existing@example.com" };
            _repositoryMock.Setup(r => r.GetByEmail("existing@example.com")).ReturnsAsync(new Customer());

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateCustomer(customer));
            Assert.Contains("cliente com esse email", ex.Message);
        }

        [Fact]
        public async Task CreateCustomer_ShouldSucceed_WhenValid()
        {
            var customer = new Customer { Name = "New", Email = "new@example.com" };
            _repositoryMock.Setup(r => r.GetByEmail("new@example.com")).ReturnsAsync((Customer)null);

            await _service.CreateCustomer(customer);

            _repositoryMock.Verify(r => r.Create(It.IsAny<Customer>()), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomer_ShouldThrowException_WhenCustomerNotFound()
        {
            var customer = new Customer { Id = 99, Name = "Update", Email = "update@example.com" };
            _repositoryMock.Setup(r => r.GetById(99)).ReturnsAsync((Customer)null);

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.UpdateCustomer(customer));
            Assert.Contains("Cliente com ID 99 não encontrado", ex.Message);
        }

        [Fact]
        public async Task DeleteCustomer_ShouldThrowException_WhenCustomerNotFound()
        {
            _repositoryMock.Setup(r => r.GetById(1)).ReturnsAsync((Customer)null);

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.DeleteCustomer(1));
            Assert.Contains("Cliente com ID 1 não encontrado", ex.Message);
        }

        [Fact]
        public async Task DeleteCustomer_ShouldCallDelete_WhenCustomerExists()
        {
            var customer = new Customer { Id = 1, Name = "To Delete", Email = "delete@example.com" };
            _repositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(customer);

            await _service.DeleteCustomer(1);

            _repositoryMock.Verify(r => r.Delete(1), Times.Once);
        }

        [Fact]
        public async Task GetAllCustomers_ShouldReturnAllCustomers()
        {
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Maria", Email = "maria@example.com" },
                new Customer { Id = 2, Name = "Carlos", Email = "carlos@example.com" }
            };

            _repositoryMock.Setup(r => r.GetAll()).ReturnsAsync(customers);

            var result = await _service.GetAllCustomers();

            Assert.Equal(2, ((List<Customer>)result).Count);
            Assert.Contains(result, c => c.Name == "Maria");
            Assert.Contains(result, c => c.Name == "Carlos");
        }

        [Fact]
        public async Task GetAllCustomers_ShouldThrowException_OnRepositoryError()
        {
            _repositoryMock.Setup(r => r.GetAll()).ThrowsAsync(new Exception("Erro de banco"));

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetAllCustomers());
            Assert.Contains("Erro de banco", ex.Message);
        }

        [Fact]
        public async Task GetCustomersByNameOrEmail_ShouldReturnFilteredCustomers()
        {
            var customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Carlos", Email = "carlos@example.com" }
            };

            _repositoryMock.Setup(r => r.GetByNameOrEmail("Carlos", "carlos@example.com"))
                .ReturnsAsync(customers);

            var result = await _service.GetCustomersByNameOrEmail("Carlos", "carlos@example.com");

            Assert.Single(result);
            Assert.Equal("Carlos", ((List<Customer>)result)[0].Name);
        }

        [Fact]
        public async Task GetCustomersByNameOrEmail_ShouldThrowException_OnRepositoryError()
        {
            _repositoryMock.Setup(r => r.GetByNameOrEmail(It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Erro ao buscar"));

            var ex = await Assert.ThrowsAsync<Exception>(() => _service.GetCustomersByNameOrEmail("Nome", "email@email.com"));
            Assert.Contains("Erro ao buscar", ex.Message);
        }
    }
}