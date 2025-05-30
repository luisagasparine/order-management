
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using OrderManagement.Models;

namespace OrderManagement.ViewModels
{
    public class NewOrderViewModel
    {
        public int CustomerId { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        [ValidateNever]
        public IEnumerable<Customer> Customers { get; set; }
        [ValidateNever]
        public IEnumerable<Product> Products { get; set; }
    }
}
