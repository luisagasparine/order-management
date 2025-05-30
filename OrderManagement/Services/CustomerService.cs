using OrderManagement.Models;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Services.Interfaces;

namespace OrderManagement.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;

        public CustomerService(ICustomerRepository customerRepository, IOrderRepository orderRepository)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
        }

        public async Task<Customer> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerRepository.GetById(id);

                if (customer == null)
                    throw new ArgumentException($"Cliente com ID {id} não encontrado.");

                return customer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Customer>> GetCustomersByNameOrEmail(string name, string email)
        {
            try
            {
                return await _customerRepository.GetByNameOrEmail(name, email);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Customer>> GetAllCustomers()
        {
            try
            {
                return await _customerRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CreateCustomer(Customer customer)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customer.Name))
                    throw new ArgumentException("O nome do cliente é obrigatório.");


                if (string.IsNullOrWhiteSpace(customer.Email) || !IsValidEmail(customer.Email))
                    throw new ArgumentException("O email fornecido é inválido.");

                var existingCustomer = await _customerRepository.GetByEmail(customer.Email);
                if (existingCustomer != null)
                    throw new ArgumentException("Já existe um cliente com esse email.");

                customer.CreatedAt = DateTime.Now;
                await _customerRepository.Create(customer);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Customer> UpdateCustomer(Customer customer)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(customer.Name))
                    throw new ArgumentException("O nome do cliente é obrigatório.");

                if (string.IsNullOrWhiteSpace(customer.Email) || !IsValidEmail(customer.Email))
                    throw new ArgumentException("O email fornecido é inválido.");

                var existingCustomer = await _customerRepository.GetById(customer.Id);
                if (existingCustomer == null)
                    throw new ArgumentException($"Cliente com ID {customer.Id} não encontrado.");


                return await _customerRepository.Update(customer);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task DeleteCustomer(int id)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetById(id);
                if (existingCustomer == null)
                    throw new ArgumentException($"Cliente com ID {id} não encontrado.");

                var orderByCustomer = await _orderRepository.GetOrderCountByCustomerId(existingCustomer.Id);
                if (orderByCustomer > 0)
                    throw new ArgumentException($"Cliente com ID {id} não pode ser excluído, pois possui pedidos vinculados.");

                await _customerRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
    }
}
