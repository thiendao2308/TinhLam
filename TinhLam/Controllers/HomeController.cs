using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TinhLam.Models;
using Microsoft.EntityFrameworkCore;
using TinhLam.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TinhLam.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TlinhContext _context;
        private async Task LoadCategories()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
        }

        // Constructor duy nhất
        public HomeController(ILogger<HomeController> logger, TlinhContext context)
        {
            _logger = logger;
            _context = context;
        }

        

        public async Task<IActionResult> Index()
        {
            await LoadCategories(); // Gọi phương thức để load danh mục

            // Lấy banner động từ database
            var banners = await _context.HomeBannerImages
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .ThenBy(b => b.ImageId)
                .ToListAsync();

            var latestProducts = await _context.Products
                .Where(p => p.IsActive == true)
                .OrderByDescending(p => p.ProductId)
                .Take(8)
                .ToListAsync();

            var bestSellingProducts = await _context.Products
                .Where(p => p.IsActive == true)
                .OrderByDescending(p => p.OrderDetails.Count) // Giả sử có quan hệ OrderDetails
                .Take(8)
                .ToListAsync();

            // Load bài viết đã publish
            var publishedPosts = await _context.Posts
                .Where(p => p.IsPublished)
                .Include(p => p.PostImages)
                .OrderByDescending(p => p.CreatedAt)
                .Take(6)
                .ToListAsync();

            ViewData["Banners"] = banners;
            ViewData["LatestProducts"] = latestProducts;
            ViewData["BestSellingProducts"] = bestSellingProducts;
            ViewData["PublishedPosts"] = publishedPosts;

            return View();
        }

        public async Task<IActionResult> DetailProduct(int id)
        {
            await LoadCategories(); // Gọi phương thức để load danh mục

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            // Lấy các sản phẩm liên quan cùng category, khác id hiện tại
            var relatedProducts = await _context.Products
                .Where(p => p.CategoryId == product.CategoryId && p.ProductId != id && p.IsActive == true)
                .OrderByDescending(p => p.ProductId)
                .Take(4)
                .ToListAsync();
            ViewBag.RelatedProducts = relatedProducts;

            return View(product);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public IActionResult DebugUser()
        {
            var userInfo = new
            {
                IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
                UserName = User.Identity?.Name,
                Roles = User.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value).ToList(),
                AllClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            };
            
            return Json(userInfo);
        }
    }
}
