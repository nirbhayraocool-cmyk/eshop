using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eshop.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal OldPrice { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        public string? Sizes { get; set; }           // "S,M,L,XL"
        public string? GalleryImages { get; set; }   // extra photos, ek line me ek

        [NotMapped]
        public List<string> SizeList =>
            string.IsNullOrWhiteSpace(Sizes)
                ? new List<string>()
                : Sizes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                       .Distinct().ToList();

        [NotMapped]
        public List<string> AllImages
        {
            get
            {
                var list = new List<string>();
                if (!string.IsNullOrWhiteSpace(ImageUrl)) list.Add(ImageUrl);
                if (!string.IsNullOrWhiteSpace(GalleryImages))
                    list.AddRange(GalleryImages.Split(new[] { '\r', '\n' },
                        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
                return list;
            }
        }
        // "Flash" ya "Featured" (Index page ka kaun sa section)
        public string Section { get; set; } = "Featured";

        public bool IsActive { get; set; } = true;

        [NotMapped]
        public int DiscountPercent =>
            OldPrice > Price && OldPrice > 0
                ? (int)Math.Round((OldPrice - Price) / OldPrice * 100)
                : 0;
    }
}