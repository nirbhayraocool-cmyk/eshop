using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eshop.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; } = 1;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}