using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TinhLam.Data;
using TinhLam.ViewModels;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly TLinhContext _context;

    public AdminController(TLinhContext context)
    {
        _context = context;
    }

    // Hiển thị danh sách danh mục
    public IActionResult AdminCategory()
    {
        var categories = _context.Categories.ToList();
        return View(categories);
    }

    public IActionResult ThongKe()
    {
        return View();
    }

    public async Task<IActionResult> GetThongKeData()
    {
        try
        {
            var revenueByMonth = await _context.Orders
                .Where(o => o.Status == "Completed")
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Month = $"{g.Key.Month}/{g.Key.Year}",
                    TotalRevenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(g => g.Month)
                .ToListAsync();

            var bestSellingProducts = await _context.OrderDetails
                .GroupBy(od => od.Product.ProductName)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalSold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(g => g.TotalSold)
                .Take(5)
                .ToListAsync();

            var bestSellingCategories = await _context.OrderDetails
                .GroupBy(od => od.Product.Category.CategoryName)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    TotalSold = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(g => g.TotalSold)
                .Take(5)
                .ToListAsync();

            return Json(new
            {
                revenueByMonth,
                bestSellingProducts,
                bestSellingCategories
            });
        }
        catch (Exception ex)
        {
            // Ghi lỗi ra console
            Console.WriteLine("Lỗi khi lấy dữ liệu thống kê: " + ex.Message);
            Console.WriteLine("StackTrace: " + ex.StackTrace);

            // Trả về lỗi 500 để dễ debug
            return StatusCode(500, "Lỗi Server: " + ex.Message);
        }
    }

}

