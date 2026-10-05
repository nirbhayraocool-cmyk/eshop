using eshop.Data;
using eshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eshop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.OrderByDescending(p => p.Id).ToListAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create() => View(new Product());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product model, IFormFile? imageFile, List<IFormFile>? galleryFiles)
        {
            ModelState.Remove(nameof(Product.ImageUrl));

            if (imageFile != null && imageFile.Length > 0)
            {
                var path = await SaveImageAsync(imageFile);
                if (path == null)
                    ModelState.AddModelError("", "The image must be in JPG, PNG, or WebP format and smaller than 2MB.");
                else
                    model.ImageUrl = path;
            }

            if (string.IsNullOrWhiteSpace(model.ImageUrl))
                ModelState.AddModelError("ImageUrl", "Upload image or enter image URL.");
            model.GalleryImages = await BuildGalleryAsync(model.GalleryImages, galleryFiles);
            if (!ModelState.IsValid) return View(model);

            model.Id = 0;
            _context.Products.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Product added.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product model, IFormFile? imageFile, List<IFormFile>? galleryFiles)
        {
            ModelState.Remove(nameof(Product.ImageUrl));

            var product = await _context.Products.FindAsync(model.Id);
            if (product == null) return NotFound();

            if (imageFile != null && imageFile.Length > 0)
            {
                var path = await SaveImageAsync(imageFile);
                if (path == null)
                    ModelState.AddModelError("", "The image must be in JPG, PNG, or WebP format and smaller than 2MB.");
                else
                    model.ImageUrl = path;
            }

            if (string.IsNullOrWhiteSpace(model.ImageUrl))
                ModelState.AddModelError("ImageUrl", "Upload image or enter image URL.");
            model.GalleryImages = await BuildGalleryAsync(model.GalleryImages, galleryFiles);
            if (!ModelState.IsValid) return View(model);

            product.Name = model.Name;
            product.Price = model.Price;
            product.OldPrice = model.OldPrice;
            product.Category = model.Category;
            product.Description = model.Description;
            product.Sizes = model.Sizes;
            product.GalleryImages = model.GalleryImages;
            product.Section = model.Section;
            product.ImageUrl = model.ImageUrl;
            product.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Product updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Product deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            var productIds = orders.SelectMany(o => o.Items).Select(i => i.ProductId).Distinct().ToList();

            ViewBag.Images = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.ImageUrl);

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int id, string status)
        {
            var allowed = new[] { "Pending", "Confirmed", "Shipped", "Delivered", "Cancelled" };
            if (!allowed.Contains(status)) return BadRequest();

            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Order #{order.Id} The status of Order  {status}  has changed to";
            }
            return RedirectToAction(nameof(Orders));
        }
        private async Task<string?> BuildGalleryAsync(string? existing, List<IFormFile>? files)
        {
            var urls = new List<string>();

            if (!string.IsNullOrWhiteSpace(existing))
                urls.AddRange(existing.Split(new[] { '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

            if (files != null)
            {
                foreach (var f in files.Where(f => f.Length > 0))
                {
                    var path = await SaveImageAsync(f);
                    if (path != null) urls.Add(path);
                }
            }

            return urls.Count == 0 ? null : string.Join("\n", urls);
        }
        private async Task<string?> SaveImageAsync(IFormFile file)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowed.Contains(ext) || file.Length > 2 * 1024 * 1024)
                return null;

            var folder = Path.Combine(_env.WebRootPath, "imgshop");
            Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + ext;
            using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
            await file.CopyToAsync(stream);

            return "/imgshop/" + fileName;
        }
    }
}