using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TinhLam.Models;
using Microsoft.EntityFrameworkCore;
using TinhLam.Data;
using System.Linq;
using System.Threading.Tasks;

namespace TinhLam.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TLinhContext _context;
        private async Task LoadCategories()
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
        }

        // Constructor duy nhất
        public HomeController(ILogger<HomeController> logger, TLinhContext context)
        {
            _logger = logger;
            _context = context;
        }

        

        public async Task<IActionResult> Index()
        {
            await LoadCategories(); // Gọi phương thức để load danh mục

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

            ViewData["LatestProducts"] = latestProducts;
            ViewData["BestSellingProducts"] = bestSellingProducts;

            return View();
        }

        public async Task<IActionResult> DetailProduct(int id)
        {
            await LoadCategories(); // Gọi phương thức để load danh mục

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

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
    }
}
