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

    // Hiển thị form thêm danh mục
    public IActionResult Index()
    {
        return View();
    }
    

}

