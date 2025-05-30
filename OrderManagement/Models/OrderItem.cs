using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O Pedido é obrigatório.")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "O Produto é obrigatório.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "A quantidade é obrigatória.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "O preço unitário é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço unitário deve ser maior que zero.")]
        public decimal UnitPrice { get; set; }

        [ValidateNever]
        public Product Product { get; set; }
    }
}
