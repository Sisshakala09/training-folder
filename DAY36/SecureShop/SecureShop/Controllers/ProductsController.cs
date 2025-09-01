using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureShop.Data;
using SecureShop.Models;
using SecureShop.ViewModels;

namespace SecureShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;
        public ProductsController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var products = await _db.Products.Include(p => p.Reviews).ThenInclude(r => r.User).ToListAsync();
            return View(products);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(ReviewVM vm)
        {
            if (!ModelState.IsValid) return RedirectToAction("Index");

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Challenge();

            var review = new Review
            {
                Content = vm.Content,
                Rating = vm.Rating,
                ProductId = vm.ProductId,
                UserId = userId
            };
            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
