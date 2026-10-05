using System.ComponentModel.DataAnnotations;

namespace eshop.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Naam likho."), StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email likho."), EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Phone, StringLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Subject likho."), StringLength(150)]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message likho."), StringLength(2000, MinimumLength = 10,
            ErrorMessage = "Message kam se kam 10 akshar ka ho.")]
        public string Message { get; set; } = string.Empty;

        // spam jaal: insaan ise nahi bharta, bots bhar dete hain
        public string? Website { get; set; }
    }
}