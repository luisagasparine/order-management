using OrderManagement.Models;
using OrderManagement.Repositories;
using OrderManagement.Repositories.Interfaces;
using OrderManagement.Services.Interfaces;
using OrderManagement.ViewModels;

namespace OrderManagement.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly INotificationStatusRepository _notificationStatusRepository;

        public OrderService(IOrderRepository orderRepository, ICustomerService customerService, IProductService productService, INotificationStatusRepository notificationStatusRepository)
        {
            _orderRepository = orderRepository;
            _customerService = customerService;
            _productService = productService;
            _notificationStatusRepository = notificationStatusRepository;
        }

        public async Task<Order> GetOrderById(int id)
        {
            try
            {
                var order = await _orderRepository.GetById(id);

                if (order == null)
                    throw new ArgumentException($"Pedido com ID {id} não encontrado.");

                return order;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerOrStatus(int? idCustomer, string status)
        {
            try
            {
                if (idCustomer == 0) { idCustomer = null; };
                return await _orderRepository.GetByCustomerOrStatus(idCustomer, status);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Order> GetOrdersDetails(int idOrder)
        {
            try
            {
                return await _orderRepository.GetDetails(idOrder);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public async Task<Order> CreateOrder(NewOrderViewModel newOrder)
        {
            try
            {
                var customer = await _customerService.GetCustomerById(newOrder.CustomerId);
                if (customer == null)
                    throw new Exception("Cliente não encontrado.");

                var orderItems = new List<OrderItem>();

                // Get item by item from the order to create the order
                foreach (var item in newOrder.Items.Where(i => i.Quantity > 0))
                {
                    var product = await _productService.GetProductById(item.ProductId);
                    if (product == null)
                        throw new Exception($"Produto com ID {item.ProductId} não encontrado.");

                    // Stock check
                    if (product.StockQuantity < item.Quantity)
                        throw new Exception($"Estoque insuficiente para o produto: {product.Name} (Disponível: {product.StockQuantity}, Solicitado: {item.Quantity})");

                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };

                    orderItems.Add(orderItem);

                    // Stock update
                    product.StockQuantity -= item.Quantity;
                    await _productService.UpdateProduct(product);
                }

                var order = new Order
                {
                    CustomerId = newOrder.CustomerId,
                    OrderDate = DateTime.Now,
                    TotalAmount = orderItems.Sum(i => i.Quantity * i.UnitPrice),
                    OrderItems = orderItems,
                    Status = StatusOrderType.Novo.ToString(),
                };

                return await _orderRepository.Create(order);
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateStatusOrder(UpdateStatusOrderViewModel updateStatus)
        {
            try
            {
                if (!Enum.IsDefined(typeof(StatusOrderType), updateStatus.Status))
                    throw new ArgumentException("O status do pedido é inválido.");

                var existingOrder = await _orderRepository.GetById(updateStatus.Id);
                if (existingOrder == null)
                    throw new ArgumentException($"Pedido com ID {updateStatus.Id} não encontrado.");

                // Checks if there was a status change and logs a notification in the database
                var oldStatus = existingOrder.Status;
                var newStatus = updateStatus.Status.ToString();

                if (oldStatus != newStatus)
                {
                    existingOrder.Status = newStatus;
                    await _orderRepository.Update(existingOrder);

                    await CreateNotification(existingOrder.Id, oldStatus, newStatus);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task CreateNotification(int orderId, string oldStatus, string newStatus)
        {
            var notification = new NotificationsStatusOrder
            {
                OrderId = orderId,
                StatusFrom = oldStatus,
                StatusTo = newStatus,
                DateChanged = DateTime.Now
            };

            await _notificationStatusRepository.CreateNotification(notification);
        }


    }
}
