using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TinhLam.Data;
using TinhLam.Models;
using TinhLam.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace TinhLam.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly TlinhContext _context;
        public CategoriesController(TlinhContext context)
        {
            _context = context;
        }
        private async Task LoadCategories()
        {
            if (_context != null)
            {
                ViewBag.Categories = await _context.Categories.ToListAsync();
            }
        }

        public IActionResult Category(int? id, int? maxPrice, string? query, int page = 1)
        {
            var productQuery = _context.Products.AsQueryable();

            if (id.HasValue)
            {
                productQuery = productQuery.Where(p => p.CategoryId == id.Value);
            }

            if (maxPrice.HasValue)
            {
                productQuery = productQuery.Where(p => p.Price <= maxPrice.Value);
            }

            // Thêm logic tìm kiếm
            if (!string.IsNullOrEmpty(query))
            {
                productQuery = productQuery.Where(p => p.ProductName.Contains(query));
            }

            // Phân trang - 12 sản phẩm mỗi trang
            int pageSize = 12;
            int skip = (page - 1) * pageSize;
            
            var totalProducts = productQuery.Count();
            var totalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            
            var products = productQuery
                .Skip(skip)
                .Take(pageSize)
                .Select(p => new ProductVM
                {
                    MaProduct = p.ProductId,
                    TenProduct = p.ProductName,
                    Price = p.Price,
                    Hinh = p.Image,
                    MoTaNgan = p.Description
                })
                .ToList();

            // Truyền thông tin phân trang vào ViewBag
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.PageSize = pageSize;
            ViewBag.CategoryId = id;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Query = query;

            return View(products);
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }


        // GET: Categories/Create
        // GET: Categories/Create
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await LoadCategories();
            return View();
        }


        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName")] Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            await LoadCategories();
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            await LoadCategories();
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
