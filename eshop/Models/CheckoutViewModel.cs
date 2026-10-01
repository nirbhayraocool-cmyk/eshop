using System.ComponentModel.DataAnnotations;

namespace eshop.Models
{
    public class CheckoutViewModel
    {
        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required, Phone]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;
    }
}