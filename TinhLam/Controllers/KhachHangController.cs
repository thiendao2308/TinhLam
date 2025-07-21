using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TinhLam.Data;
using TinhLam.Helpers;
using TinhLam.ViewModels;

namespace TinhLam.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly TlinhContext db;
        private readonly IMapper _mapper;
        public KhachHangController(TlinhContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult DangKy()
        {
            return View(new RegisterVM()); 
        }

        [HttpPost]
        public IActionResult DangKy(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var khachHang = _mapper.Map<User>(model);
                khachHang.RandomKey = MyUtil.GenerateRandomKey();
                var passwordHasher = new PasswordHasher<User>();
                khachHang.Password = passwordHasher.HashPassword(khachHang, model.Password);
                khachHang.Role = "User";
                khachHang.HieuLuc = true;
                khachHang.Diachi = model.DiaChi;
                db.Add(khachHang);
                db.SaveChanges();
                
            }
            return RedirectToAction("DangNhap", "KhachHang");
        }

        [HttpGet]
        public IActionResult DangNhap(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View(new LoginVM());
        }

        [HttpPost]
        public async Task<IActionResult> DangNhap(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var khachHang = db.Users.SingleOrDefault(kh => kh.Username == model.UserName);

            // ✅ Nếu không tìm thấy tài khoản hoặc tài khoản bị khóa
            if (khachHang == null || !khachHang.HieuLuc)
            {
                ModelState.AddModelError("lỗi", "Tài khoản hoặc mật khẩu không chính xác.");
                return View(model);
            }

            var passwordHasher = new PasswordHasher<User>();
            if (passwordHasher.VerifyHashedPassword(khachHang, khachHang.Password, model.Password) != PasswordVerificationResult.Success)
            {
                ModelState.AddModelError("lỗi", "Tài khoản hoặc mật khẩu không chính xác.");
                return View(model);
            }

            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, khachHang.Email),
                new Claim(ClaimTypes.Name, khachHang.CustomerName),
                new Claim(MySetting.CLAIM_CUSTOMERID, khachHang.UserId.ToString()),
                new Claim(ClaimTypes.Role, khachHang.Role), // Phân quyền User/Admin
                new Claim(ClaimTypes.NameIdentifier, khachHang.UserId.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false,  // Không lưu cookie sau khi đóng trình duyệt
                ExpiresUtc = DateTime.UtcNow.AddHours(1) // Tùy chỉnh thời gian hết hạn trong phiên
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authProperties);

            
            if (khachHang.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("ThongKe", "Admin");
            }

            if (Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }

            return Redirect("/");
        }


        [Authorize(Roles = "User,Admin")]
        public IActionResult Profile(string status, DateOnly? startDate, DateOnly? endDate)
        {
            var userIdClaim = User.FindFirst(MySetting.CLAIM_CUSTOMERID);
            if (userIdClaim == null)
            {
                return RedirectToAction("DangNhap");
            }

            int userId = int.Parse(userIdClaim.Value);
            var khachHang = db.Users.FirstOrDefault(kh => kh.UserId == userId);
            if (khachHang == null)
            {
                return NotFound();
            }

            var ordersQuery = db.Orders.Where(o => o.UserId == userId);

            // Lọc theo trạng thái nếu có
            if (!string.IsNullOrEmpty(status))
            {
                ordersQuery = ordersQuery.Where(o => o.Status == status);
            }

            // Lọc theo ngày nếu có
            if (startDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate <= endDate.Value);
            }

            var orders = ordersQuery.OrderByDescending(o => o.OrderDate).ToList();

            var profileVM = new ProfileVM
            {
                UserId = khachHang.UserId,
                CustomerName = khachHang.CustomerName,
                Email = khachHang.Email,
                PhoneNumber = khachHang.PhoneNumber,
                Diachi = khachHang.Diachi,
                RewardPoints = khachHang.RewardPoints,
                Orders = orders
            };

            return View(profileVM);
        }




        [Authorize] // Áp dụng cho mọi tài khoản đã đăng nhập (User và Admin)
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync();

            // Nếu là admin, chuyển về trang đăng nhập admin
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("DangNhap", "KhachHang");
            }

            // Nếu là user bình thường, chuyển về trang chủ
            return Redirect("/");
        }



    }
}
