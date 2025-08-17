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
using TinhLam.Helpers;
using Microsoft.Extensions.Configuration;

namespace TinhLam.Controllers
{
    public class ProductsController : Controller
    {
        private readonly TlinhContext _context;
        private readonly IConfiguration _configuration;


        public ProductsController(TlinhContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        

        // GET: Products
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var tlinhContext = _context.Products.Include(p => p.Category).Include(p => p.ProductSizes);
            return View(await tlinhContext.ToListAsync());
        }

        // GET: Products/Details/5
        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductSizes)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Category = product.Category;
            return View(product);
        }

        public IActionResult Search(string? query)
        {
            return RedirectToAction("Category", "Categories", new { query = query });
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Price,StockQuantity,Description,SizeOption,CategoryId,IsActive")] Product product, IFormFile ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Upload ảnh lên Cloudinary
                    var imageUrl = MyUtil.UploadImageToCloudinary(ImageFile, _configuration, "products");
                    product.Image = imageUrl;
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,Price,Description,Image,CategoryId,IsActive")] Product product, IFormFile ImageFile)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các trường thông tin
                    existingProduct.ProductName = product.ProductName;
                    existingProduct.Price = product.Price;
                    // existingProduct.StockQuantity = product.StockQuantity;
                    existingProduct.Description = product.Description;
                    // existingProduct.SizeOption = product.SizeOption;
                    existingProduct.CategoryId = product.CategoryId;
                    existingProduct.IsActive = product.IsActive;

                    // Xử lý upload ảnh mới nếu có
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // Upload ảnh mới lên Cloudinary
                        var imageUrl = MyUtil.UploadImageToCloudinary(ImageFile, _configuration, "products");
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            existingProduct.Image = imageUrl;
                        }
                    }
                    // Nếu không upload ảnh mới thì giữ nguyên ảnh cũ

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    var product = await _context.Products
        .Include(p => p.ProductSizes)
        .FirstOrDefaultAsync(p => p.ProductId == id);

    Console.WriteLine($"[Delete] Đang kiểm tra sản phẩm ID: {id}");

    bool hasPendingOrder = await _context.OrderDetails
        .Include(od => od.Order)
        .AnyAsync(od => od.ProductId == id && od.Order.Status != "Completed");

    Console.WriteLine($"[Delete] hasPendingOrder: {hasPendingOrder}");

    if (hasPendingOrder)
    {
        TempData["ErrorMessage"] = "Không thể xóa sản phẩm vì đang có đơn hàng chưa hoàn thành liên quan!";
        return RedirectToAction(nameof(Index));
    }

    if (product.ProductSizes != null)
        _context.ProductSizes.RemoveRange(product.ProductSizes);

    _context.Products.Remove(product);
    await _context.SaveChangesAsync();
    TempData["SuccessMessage"] = "Đã xóa sản phẩm thành công!";
    return RedirectToAction(nameof(Index));
}

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        // GET: Products/ManageSizes/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageSizes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductSizes)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/AddSize/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSize(int? productId)
        {
            if (productId == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.ProductId = productId;
            ViewBag.ProductName = product.ProductName;
            
            // Tạo model rỗng để tránh NullReferenceException
            var productSize = new ProductSize();
            return View(productSize);
        }

        // POST: Products/AddSize
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSize(int productId, string size, int stockQuantity)
        {
            // Log chi tiết để debug
            Console.WriteLine("=== ADD SIZE DEBUG ===");
            Console.WriteLine($"ProductId: {productId}");
            Console.WriteLine($"Size: '{size}'");
            Console.WriteLine($"StockQuantity: {stockQuantity}");
            
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(size))
            {
                ModelState.AddModelError("Size", "Size không được để trống");
            }
            
            if (stockQuantity < 0)
            {
                ModelState.AddModelError("StockQuantity", "Số lượng phải lớn hơn hoặc bằng 0");
            }
            
            if (!ModelState.IsValid)
            {
                Console.WriteLine("=== VALIDATION FAILED ===");
                var product = await _context.Products.FindAsync(productId);
                ViewBag.ProductId = productId;
                ViewBag.ProductName = product?.ProductName;
                
                // Tạo model để trả về view
                var productSize = new ProductSize
                {
                    Size = size,
                    StockQuantity = stockQuantity
                };
                return View(productSize);
            }

            try
            {
                Console.WriteLine("=== ATTEMPTING TO SAVE ===");
                
                // Tạo object mới với ProductId được set
                var newProductSize = new ProductSize
                {
                    ProductId = productId,
                    Size = size.Trim(),
                    StockQuantity = stockQuantity
                };
                
                Console.WriteLine($"New ProductSize - ProductId: {newProductSize.ProductId}, Size: '{newProductSize.Size}', StockQuantity: {newProductSize.StockQuantity}");
                
                _context.ProductSizes.Add(newProductSize);
                var result = await _context.SaveChangesAsync();
                
                Console.WriteLine($"SaveChanges result: {result} rows affected");
                Console.WriteLine("=== SAVE SUCCESSFUL ===");
                
                return RedirectToAction(nameof(ManageSizes), new { id = productId });
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== EXCEPTION OCCURRED ===");
                Console.WriteLine($"Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                
                var product = await _context.Products.FindAsync(productId);
                ViewBag.ProductId = productId;
                ViewBag.ProductName = product?.ProductName;
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
                
                // Tạo model để trả về view
                var productSize = new ProductSize
                {
                    Size = size,
                    StockQuantity = stockQuantity
                };
                return View(productSize);
            }
        }

        // GET: Products/EditSize/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditSize(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productSize = await _context.ProductSizes
                .Include(ps => ps.Product)
                .FirstOrDefaultAsync(ps => ps.ProductSizeId == id);

            if (productSize == null)
            {
                return NotFound();
            }

            // Đảm bảo Product được load
            if (productSize.Product == null)
            {
                productSize.Product = await _context.Products.FindAsync(productSize.ProductId);
            }

            return View(productSize);
        }

        // POST: Products/EditSize/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditSize(int id, [Bind("ProductSizeId,ProductId,Size,StockQuantity")] ProductSize productSize)
        {
            if (id != productSize.ProductSizeId)
            {
                return NotFound();
            }
            ModelState.Remove("Product");
            if (ModelState.IsValid)
            {
                // Lấy entity có tracking từ context
                var entity = await _context.ProductSizes
                    .Where(ps => ps.ProductSizeId == id)
                    .FirstOrDefaultAsync();

                if (entity == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy size cần sửa trong database.");
                    if (productSize.Product == null)
                        productSize.Product = await _context.Products.FindAsync(productSize.ProductId);
                    return View(productSize);
                }

                // Cập nhật các trường cần thiết
                entity.Size = productSize.Size;
                entity.StockQuantity = productSize.StockQuantity;

                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(ManageSizes), new { id = entity.ProductId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Lỗi khi lưu dữ liệu: " + ex.Message);
                }
            }

            
            if (productSize.Product == null)
                productSize.Product = await _context.Products.FindAsync(productSize.ProductId);
            return View(productSize);
        }

        // POST: Products/DeleteSize/5
        [HttpPost, ActionName("DeleteSize")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSizeConfirmed(int id)
        {
            var productSize = await _context.ProductSizes.FindAsync(id);
            if (productSize != null)
            {
                var productId = productSize.ProductId;
                _context.ProductSizes.Remove(productSize);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageSizes), new { id = productId });
            }
            return NotFound();
        }

        private bool ProductSizeExists(int id)
        {
            return _context.ProductSizes.Any(e => e.ProductSizeId == id);
        }
    }
}
