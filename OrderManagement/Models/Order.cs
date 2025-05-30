using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O Cliente é obrigatório.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "A data do pedido é obrigatória.")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "O valor total é obrigatório.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor total deve ser maior que zero.")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "O status do pedido é obrigatório.")]
        [StringLength(50, ErrorMessage = "O status não pode ter mais de 50 caracteres.")]
        public string Status { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new();
        public Customer Customer { get; set; }

    }
}
