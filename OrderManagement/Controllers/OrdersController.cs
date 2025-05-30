using Microsoft.AspNetCore.Mvc;
using OrderManagement.Models;
using OrderManagement.Services.Interfaces;
using OrderManagement.ViewModels;

namespace OrderManagement.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public OrdersController(ICustomerService customerService, IOrderService orderService, IProductService productService) {
            _customerService = customerService;
            _orderService = orderService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var customer = await _customerService.GetAllCustomers();
            return View(customer);
        }

        public async Task<IActionResult> CreateOrder()
        {
            var customers = await _customerService.GetAllCustomers();
            var products = await _productService.GetAllProducts();

            var viewModel = new NewOrderViewModel
            {
                Customers = customers,
                Products = products
            };

            return View(viewModel);
        }

        public async Task<IActionResult> DetailsOrder(int id)
        {
            var orderDetails = await _orderService.GetOrdersDetails(id);
            return View(orderDetails);
        }

        public IActionResult ConfirmationOrder(int id)
        {
            return View("ConfirmationOrder", id);
        }
    }
}
