using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models
{
    public class NotificationsStatusOrder
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "O status de origem não pode ter mais de 50 caracteres.")]
        public string StatusFrom { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "O status de destino não pode ter mais de 50 caracteres.")]
        public string StatusTo { get; set; }

        [Required]
        public DateTime DateChanged { get; set; }
    }
}
