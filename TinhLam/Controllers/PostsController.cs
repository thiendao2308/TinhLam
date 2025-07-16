using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TinhLam.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IO;
using TinhLam.Models;
namespace TinhLam.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PostsController : Controller
    {
        private readonly TlinhContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostsController(TlinhContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Include(p => p.PostComments)
                .Include(p => p.PostImages)
                .AsSplitQuery()
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(posts);
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.PostComments)
                    .ThenInclude(c => c.User)
                .Include(p => p.PostImages)
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,Author,Tags,IsPublished,PostedBy")] Post post, List<IFormFile> images)
        {
            // Log để debug
            Console.WriteLine($"Create Post - Title: {post.Title}, Content Length: {post.Content?.Length ?? 0}");
            
            // Set PostedBy trước khi validation
            post.PostedBy = User.Identity.Name ?? "Admin";
            post.CreatedAt = DateTime.Now;
            post.ViewCount = 0;
            
            // Clear PostedBy validation error nếu có
            ModelState.Remove("PostedBy");
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine($"ModelState Errors: {string.Join(", ", errors)}");
                return View(post);
            }

            try
            {
                // Xử lý ảnh chính
                if (images != null && images.Count > 0)
                {
                    var mainImage = images.FirstOrDefault();
                    if (mainImage != null && mainImage.Length > 0)
                    {
                        Console.WriteLine($"Processing main image: {mainImage.FileName}, Size: {mainImage.Length}");
                        var fileName = await SaveImage(mainImage);
                        post.Image = fileName;
                        Console.WriteLine($"Saved image as: {fileName}");
                    }
                }

                _context.Add(post);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Post saved with ID: {post.PostId}");

                // Xử lý các ảnh phụ (bỏ qua ảnh đầu tiên vì đã xử lý ở trên)
                if (images != null && images.Count > 1)
                {
                    for (int i = 1; i < images.Count; i++)
                    {
                        var image = images[i];
                        if (image != null && image.Length > 0)
                        {
                            Console.WriteLine($"Processing additional image {i}: {image.FileName}");
                            var fileName = await SaveImage(image);
                            var postImage = new PostImage
                            {
                                PostId = post.PostId,
                                ImageUrl = fileName,
                                DisplayOrder = i
                            };
                            _context.PostImages.Add(postImage);
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Bài viết đã được tạo thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating post: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", $"Có lỗi xảy ra khi tạo bài viết: {ex.Message}");
                return View(post);
            }
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.PostImages)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Posts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Content,Author,Tags,IsPublished")] Post post, List<IFormFile> images)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPost = await _context.Posts.FindAsync(id);
                    if (existingPost == null)
                    {
                        return NotFound();
                    }

                    existingPost.Title = post.Title;
                    existingPost.Content = post.Content;
                    existingPost.Author = post.Author;
                    existingPost.Tags = post.Tags;
                    existingPost.IsPublished = post.IsPublished;

                    // Xử lý ảnh mới nếu có
                    if (images != null && images.Count > 0)
                    {
                        var mainImage = images.FirstOrDefault();
                        if (mainImage != null && mainImage.Length > 0)
                        {
                            var fileName = await SaveImage(mainImage);
                            existingPost.Image = fileName;
                        }
                    }

                    _context.Update(existingPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
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
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts
                .Include(p => p.PostComments)
                .Include(p => p.PostImages)
                .FirstOrDefaultAsync(p => p.PostId == id);

            if (post != null)
            {
                // Xóa comments
                _context.PostComments.RemoveRange(post.PostComments);
                
                // Xóa images
                _context.PostImages.RemoveRange(post.PostImages);
                
                // Xóa post
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Posts/ViewPost/5 (cho người dùng xem bài viết)
        [AllowAnonymous]
        public async Task<IActionResult> ViewPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.PostComments.OrderByDescending(c => c.CommentDate))
                    .ThenInclude(c => c.User)
                .Include(p => p.PostImages.OrderBy(i => i.DisplayOrder))
                .AsSplitQuery()
                .FirstOrDefaultAsync(m => m.PostId == id && m.IsPublished);

            if (post == null)
            {
                return NotFound();
            }

            // Tăng view count
            post.ViewCount++;
            _context.Update(post);
            await _context.SaveChangesAsync();

            return View(post);
        }

        // GET: Posts/List (cho người dùng xem danh sách bài viết)
        [AllowAnonymous]
        public async Task<IActionResult> List()
        {
            var posts = await _context.Posts
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PostListItemViewModel
                {
                    PostId = p.PostId,
                    Title = p.Title,
                    Author = p.Author,
                    CreatedAt = p.CreatedAt,
                    ViewCount = p.ViewCount,
                    Image = p.Image,
                    Tags = p.Tags,
                    CommentCount = p.PostComments.Count
                })
                .ToListAsync();

            return View(posts);
        }


        // POST: Posts/AddComment
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> AddComment(int postId, string commentContent, string phoneNumber)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(commentContent?.Trim()))
                {
                    return Json(new { success = false, message = "Nội dung bình luận không được để trống" });
                }

                if (commentContent.Length > 1000)
                {
                    return Json(new { success = false, message = "Nội dung bình luận không được quá 1000 ký tự" });
                }

                // Kiểm tra bài viết có tồn tại và đã xuất bản không
                var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId && p.IsPublished);
                if (post == null)
                {
                    return Json(new { success = false, message = "Bài viết không tồn tại hoặc chưa được xuất bản" });
                }

                var comment = new PostComment
                {
                    PostId = postId,
                    CommentContent = commentContent.Trim(),
                    PhoneNumber = phoneNumber?.Trim() ?? "",
                    CommentDate = DateTime.Now
                };

                // Nếu user đã đăng nhập, lấy UserId
                if (User.Identity.IsAuthenticated)
                {
                    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        comment.UserId = userId;
                    }
                }

                _context.PostComments.Add(comment);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Bình luận đã được thêm thành công" });
            }
            catch (Exception ex)
            {
                // Log lỗi (có thể thêm logging service sau)
                Console.WriteLine($"Error adding comment: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi thêm bình luận. Vui lòng thử lại!" });
            }
        }

        // POST: Posts/DeleteComment
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            try
            {
                var comment = await _context.PostComments.FindAsync(commentId);
                if (comment == null)
                {
                    return Json(new { success = false, message = "Bình luận không tồn tại" });
                }

                _context.PostComments.Remove(comment);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Bình luận đã được xóa thành công" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting comment: {ex.Message}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa bình luận. Vui lòng thử lại!" });
            }
        }

        // GET: Posts/Comments (quản lý bình luận cho admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Comments()
        {
            var comments = await _context.PostComments
                .Include(c => c.Post)
                .Include(c => c.User)
                .OrderByDescending(c => c.CommentDate)
                .ToListAsync();

            return View(comments);
        }

        // GET: Posts/GetCommentsByPost
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCommentsByPost(int postId)
        {
            try
            {
                var comments = await _context.PostComments
                    .Include(c => c.User)
                    .Where(c => c.PostId == postId)
                    .OrderByDescending(c => c.CommentDate)
                    .Select(c => new
                    {
                        commentId = c.CommentId,
                        commentContent = c.CommentContent,
                        commentDate = c.CommentDate,
                        phoneNumber = c.PhoneNumber,
                        userId = c.UserId,
                        userName = c.User != null ? c.User.CustomerName : null
                    })
                    .ToListAsync();

                return Json(new { success = true, comments = comments });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi tải bình luận: " + ex.Message });
            }
        }

        // GET: Posts/Test
        public async Task<IActionResult> Test()
        {
            try
            {
                var postCount = await _context.Posts.CountAsync();
                var canConnect = await _context.Database.CanConnectAsync();
                
                return Json(new { 
                    success = true, 
                    postCount = postCount, 
                    canConnect = canConnect,
                    message = "Database connection successful"
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        // GET: Posts/TestFile
        public IActionResult TestFile()
        {
            try
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "posts");
                var testFile = Path.Combine(uploadsFolder, "test.txt");
                
                // Test ghi file
                System.IO.File.WriteAllText(testFile, "Test write permission");
                
                // Test đọc file
                var content = System.IO.File.ReadAllText(testFile);
                
                // Xóa file test
                System.IO.File.Delete(testFile);
                
                return Json(new { 
                    success = true, 
                    uploadsFolder = uploadsFolder,
                    canWrite = true,
                    canRead = true,
                    message = "File permissions test successful"
                });
            }
            catch (Exception ex)
            {
                return Json(new { 
                    success = false, 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return string.Empty;

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "posts");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return uniqueFileName;
        }
    }
} 