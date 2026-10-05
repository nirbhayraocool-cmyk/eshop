using eshop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eshop.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ContactMessage> ContactMessages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);   // Identity ke liye zaroori

            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Shirts",
                    Price = 1999,
                    OldPrice = 3332,
                    Section = "Flash",
                    Category = "Fashion",
                    ImageUrl = "https://www.shutterstock.com/image-photo/asian-beautiful-young-women-look-260nw-2508426961.jpg"
                },
                new Product
                {
                    Id = 2,
                    Name = "Shirts",
                    Price = 1499,
                    OldPrice = 2141,
                    Section = "Flash",
                    Category = "Fashion",
                    ImageUrl = "https://thumbs.dreamstime.com/b/near-clothes-rack-showcase-girls-buy-something-summer-wardrobe-choose-some-items-discount-season-promotional-offers-client-465644853.jpg"
                },
                new Product
                {
                    Id = 3,
                    Name = "Fashions",
                    Price = 999,
                    OldPrice = 1332,
                    Section = "Flash",
                    Category = "Fashion",
                    ImageUrl = "https://www.livemint.com/lm-img/img/2024/10/03/original/pe_1727956111669.jpg"
                },

                new Product { Id = 4, Name = "Shoes", Price = 1999, OldPrice = 2665, Section = "Featured", Category = "Shoes", ImageUrl = "/imgshop/boot.webp" },
                new Product { Id = 5, Name = "Shoes", Price = 1999, OldPrice = 2665, Section = "Featured", Category = "Shoes", ImageUrl = "/imgshop/boot-2.webp" },
                new Product { Id = 6, Name = "Shoes", Price = 1999, OldPrice = 2665, Section = "Featured", Category = "Shoes", ImageUrl = "/imgshop/boot-3.jpg" },
                new Product { Id = 7, Name = "Shoes", Price = 1999, OldPrice = 2665, Section = "Featured", Category = "Shoes", ImageUrl = "/imgshop/boot4.jpg" }
            );
        }
    }
}