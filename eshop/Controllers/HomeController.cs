

using System.Diagnostics;
using eshop.Data;
using eshop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace eshop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var vm = new HomeViewModel
            {
                FlashProducts = await _context.Products
                    .Where(p => p.IsActive && p.Section == "Flash").ToListAsync(),
                FeaturedProducts = await _context.Products
                    .Where(p => p.IsActive && p.Section == "Featured").ToListAsync()
            };
            return View(vm);
        }
        public async Task<IActionResult> Search(string? q)
        {
            q = q?.Trim();

            var showAll = string.IsNullOrEmpty(q) || q.Equals("all", StringComparison.OrdinalIgnoreCase);

            var query = _context.Products.Where(p => p.IsActive);

            if (!showAll)
            {
                query = query.Where(p => p.Name.Contains(q!) || p.Category.Contains(q!));
            }

            var products = await query.OrderBy(p => p.Name).ToListAsync();

            ViewBag.Query = showAll ? null : q;
            return View(products);
        }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null) return NotFound();

            ViewBag.Related = await _context.Products
                .Where(p => p.IsActive && p.Id != id && p.Category == product.Category)
                .Take(4)
                .ToListAsync();

            return View(product);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1, string? size = null, string? returnUrl = null)
        {
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (quantity < 1) quantity = 1;
            if (quantity > 100) quantity = 100;

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);

            if (product == null)
            {
                if (isAjax) return Json(new { success = false, message = "Product not found." });

                TempData["Error"] = "Product not found.";
                return RedirectBack(returnUrl);
            }

            // jis product ke sizes hain, uske liye size chunna zaroori hai
            var sizes = product.SizeList;
            if (sizes.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(size) || !sizes.Contains(size))
                {
                    const string msg = "Pehle size chuno.";
                    if (isAjax) return Json(new { success = false, message = msg });

                    TempData["Error"] = msg;
                    return RedirectToAction("Details", new { id = productId });
                }
            }
            else
            {
                size = null;
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // same product + same size ho to quantity badhao, warna naya row
            var existing = await _context.CartItems
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId && x.Size == size);

            if (existing != null)
            {
                existing.Quantity = Math.Min(existing.Quantity + quantity, 100);
            }
            else
            {
                _context.CartItems.Add(new CartItem
                {
                    UserId = userId,
                    ProductId = product.Id,
                    ProductName = product.Name,   // DB se
                    Price = product.Price,        // DB se
                    Size = size,
                    Quantity = quantity,
                    CreatedAt = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();

            var okMessage = size == null
                ? $"{product.Name} cart me add ho gaya!"
                : $"{product.Name} (Size {size}) cart me add ho gaya!";

            if (isAjax)
            {
                var cartCount = await _context.CartItems
                    .Where(x => x.UserId == userId)
                    .SumAsync(x => x.Quantity);

                return Json(new { success = true, message = okMessage, cartCount });
            }

            TempData["Success"] = okMessage;
            return RedirectBack(returnUrl);
        }
        private IActionResult RedirectBack(string? returnUrl) =>
            !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? LocalRedirect(returnUrl)
                : RedirectToAction("Index");

        [Authorize]
        [Authorize]
        public async Task<IActionResult> Cart()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var items = await _context.CartItems
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            ViewBag.LastOrder = await _context.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            return View(items);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            if (quantity < 1) quantity = 1;
            if (quantity > 100) quantity = 100;

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await _context.CartItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (item != null)
            {
                item.Quantity = quantity;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Cart");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var item = await _context.CartItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Cart");
        }
        [HttpGet]
        public IActionResult Contact()
        {
            var model = new ContactViewModel();

            // login hai to apna email pehle se bhar do
            if (User.Identity?.IsAuthenticated == true)
                model.Email = User.Identity.Name ?? string.Empty;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(ContactViewModel model)
        {
            // honeypot bhara hai to bot hai: chupchap success dikha do
            if (!string.IsNullOrEmpty(model.Website))
            {
                TempData["ContactSuccess"] = "Aapka message bhej diya gaya. Hum jaldi reply karenge.";
                return RedirectToAction(nameof(Contact));
            }

            if (!ModelState.IsValid) return View(model);

            // ek email se 10 minute me 3 se zyada message nahi
            var since = DateTime.Now.AddMinutes(-10);
            var recent = await _context.ContactMessages
                .CountAsync(m => m.Email == model.Email && m.CreatedAt > since);

            if (recent >= 3)
            {
                ModelState.AddModelError("", "Bahut zyada messages bheje ja chuke hain. Kuch der baad try karo.");
                return View(model);
            }

            var msg = new ContactMessage
            {
                Name = model.Name.Trim(),
                Email = model.Email.Trim(),
                Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim(),
                Subject = model.Subject.Trim(),
                Message = model.Message.Trim(),
                CreatedAt = DateTime.Now
            };

            _context.ContactMessages.Add(msg);
            await _context.SaveChangesAsync();

            TempData["ContactSuccess"] = "Your message has been sent. We will reply soon.";
            return RedirectToAction(nameof(Contact));
        }
        
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}