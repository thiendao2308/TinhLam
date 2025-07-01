using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TinhLam.Data;
using Microsoft.AspNetCore.Authorization;

namespace TinhLam.Controllers
{
    [Authorize(Roles = "Admin")]
    public class HomeBannerController : Controller
    {
        private readonly TlinhContext _context;

        public HomeBannerController(TlinhContext context)
        {
            _context = context;
        }

        // GET: HomeBanner
        public async Task<IActionResult> Index()
        {
            var banners = await _context.HomeBannerImages
                .OrderBy(b => b.DisplayOrder)
                .ThenBy(b => b.ImageId)
                .ToListAsync();
            return View(banners);
        }

        // GET: HomeBanner/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HomeBanner/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Caption,Link,DisplayOrder,IsActive")] HomeBannerImage homeBannerImage, IFormFile ImageFile)
        {
            // Kiểm tra file ảnh
            if (ImageFile == null || ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Vui lòng chọn file ảnh");
                return View(homeBannerImage);
            }

            // Kiểm tra loại file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("ImageFile", "Chỉ chấp nhận file ảnh: JPG, JPEG, PNG, GIF");
                return View(homeBannerImage);
            }

            try
            {
                // Tạo tên file duy nhất
                var fileName = Guid.NewGuid().ToString() + fileExtension;

                // Đảm bảo thư mục lưu ảnh tồn tại
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/banners");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Đường dẫn đầy đủ để lưu ảnh
                var filePath = Path.Combine(uploadPath, fileName);

                // Lưu file ảnh
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                // Clear ModelState cho ImageUrl trước khi set giá trị mới
                ModelState.Remove("ImageUrl");
                
                // Set các giá trị cho model
                homeBannerImage.ImageUrl = "images/banners/" + fileName;
                homeBannerImage.CreatedAt = DateTime.Now;

                // Kiểm tra validation sau khi đã set đầy đủ giá trị
                if (ModelState.IsValid)
                {
                    _context.Add(homeBannerImage);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thêm banner thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                // Log lỗi
                ModelState.AddModelError("", $"Lỗi khi thêm banner: {ex.Message}");
            }

            // Nếu có lỗi validation, hiển thị lại form với lỗi
            return View(homeBannerImage);
        }

        // GET: HomeBanner/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var homeBannerImage = await _context.HomeBannerImages.FindAsync(id);
            if (homeBannerImage == null)
            {
                return NotFound();
            }
            return View(homeBannerImage);
        }

        // POST: HomeBanner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ImageId,ImageUrl,Caption,Link,DisplayOrder,IsActive,CreatedAt")] HomeBannerImage homeBannerImage, IFormFile ImageFile)
        {
            if (id != homeBannerImage.ImageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBanner = await _context.HomeBannerImages.FindAsync(id);
                    if (existingBanner == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các trường thông tin
                    existingBanner.Caption = homeBannerImage.Caption;
                    existingBanner.Link = homeBannerImage.Link;
                    existingBanner.DisplayOrder = homeBannerImage.DisplayOrder;
                    existingBanner.IsActive = homeBannerImage.IsActive;

                    // Xử lý upload ảnh mới nếu có
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // Xóa ảnh cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(existingBanner.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingBanner.ImageUrl);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Tạo tên file duy nhất
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/banners");
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }
                        var filePath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }
                        existingBanner.ImageUrl = "images/banners/" + fileName;
                    }

                    _context.Update(existingBanner);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật banner thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HomeBannerImageExists(homeBannerImage.ImageId))
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
            return View(homeBannerImage);
        }

        // GET: HomeBanner/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var homeBannerImage = await _context.HomeBannerImages
                .FirstOrDefaultAsync(m => m.ImageId == id);
            if (homeBannerImage == null)
            {
                return NotFound();
            }

            return View(homeBannerImage);
        }

        // POST: HomeBanner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var homeBannerImage = await _context.HomeBannerImages.FindAsync(id);
            if (homeBannerImage != null)
            {
                // Xóa file ảnh nếu tồn tại
                if (!string.IsNullOrEmpty(homeBannerImage.ImageUrl))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", homeBannerImage.ImageUrl);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.HomeBannerImages.Remove(homeBannerImage);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Xóa banner thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool HomeBannerImageExists(int id)
        {
            return _context.HomeBannerImages.Any(e => e.ImageId == id);
        }
    }
} 