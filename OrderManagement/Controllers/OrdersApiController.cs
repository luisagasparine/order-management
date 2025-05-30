using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Services.Interfaces;
using OrderManagement.ViewModels;

namespace OrderManagement.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersApiController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public OrdersApiController(ICustomerService customerService, IOrderService orderService, IProductService productService)
        {
            _customerService = customerService;
            _orderService = orderService;
            _productService = productService;
        }

        // GET: api/orders?idCustomer={idCustomer}&status={status}
        [HttpGet]
        public async Task<ActionResult> GetOrders(int? idCustomer, string? status)
        {
            try
            {
                var orders = await _orderService.GetOrdersByCustomerOrStatus(idCustomer, status);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao recuperar os pedidos: {ex.Message}" });
            }
        }

        // GET: api/orders/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderById(id);
                if (order == null)
                    return NotFound(new { message = $"Pedido com ID {id} não encontrado." });

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao recuperar o pedido: {ex.Message}" });
            }
        }

        // PUT: api/orders/
        [HttpPut("status")]
        public async Task<ActionResult> UpdateStatusOrder([FromBody] UpdateStatusOrderViewModel updateStatusOrder)
        {
            try
            {
                await _orderService.UpdateStatusOrder(updateStatusOrder);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao atualizar o pedido: {ex.Message}" });
            }
        }

        // POST: api/orders/
        [HttpPost]
        public async Task<IActionResult> CreateOrder(NewOrderViewModel model)
        {
            var validItems = model.Items.Where(i => i.Quantity > 0).ToList();

            if (!validItems.Any())
                ModelState.AddModelError("", "Selecione ao menos um produto.");

            if (!ModelState.IsValid)
                return BadRequest(new { message = "Os dados do pedido são inválidos." });

            model.Items = validItems;
            var newOrder = await _orderService.CreateOrder(model);
            return Ok(new { id = newOrder.Id });
        }



    }
}
