using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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



    
    public async Task<IActionResult> ThongKe(string selectedDate)
    {
        DateTime? dateFilter = null;
        if (!string.IsNullOrEmpty(selectedDate))
        {
            dateFilter = DateTime.ParseExact(selectedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        var revenueQuery = _context.Orders.AsQueryable();

        if (dateFilter.HasValue)
        {
            DateOnly dateOnlyFilter = DateOnly.FromDateTime(dateFilter.Value);
            revenueQuery = revenueQuery.Where(o => o.OrderDate == dateOnlyFilter);
        }


        var revenueData = await revenueQuery
            .GroupBy(o => o.OrderDate)
            .Select(g => new RevenueVM
            {
                Date = g.Key,
                TotalRevenue = g.Sum(o => o.TotalAmount)
            })
            .OrderByDescending(r => r.Date)
            .ToListAsync();

        return View(revenueData);
    }
}