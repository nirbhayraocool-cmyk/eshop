namespace eshop.Models
{
    public class HomeViewModel
    {
        public List<Product> FlashProducts { get; set; } = new();
        public List<Product> FeaturedProducts { get; set; } = new();
    }
}