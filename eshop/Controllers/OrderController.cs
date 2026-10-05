using System.Security.Claims;
using eshop.Data;
using eshop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eshop.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Fill in the name, phone number, and address correctly.";
                return RedirectToAction("Cart", "Home");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var cartItems = await _context.CartItems
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Error"] = "The cart is empty.";
                return RedirectToAction("Cart", "Home");
            }

            var order = new Order
            {
                UserId = userId,
                CustomerName = model.CustomerName,
                Phone = model.Phone,
                Address = model.Address,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                TotalAmount = cartItems.Sum(x => x.Price * x.Quantity),
                Items = cartItems.Select(x => new OrderItem
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    
                    Size = x.Size
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);   // order ke baad cart khali
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Order #{order.Id} has been placed";
            return RedirectToAction(nameof(MyOrders));
        }

        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var orders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }
    }
}