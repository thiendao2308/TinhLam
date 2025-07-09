using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinhLam.Data;
using TinhLam.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace TinhLam.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RewardPointsController : Controller
    {
        private readonly TlinhContext _context;
        private readonly IRewardPointService _rewardPointService;

        public RewardPointsController(TlinhContext context, IRewardPointService rewardPointService)
        {
            _context = context;
            _rewardPointService = rewardPointService;
        }

        // GET: RewardPoints
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users
                .Where(u => u.RewardPoints > 0)
                .OrderByDescending(u => u.RewardPoints)
                .ToListAsync();

            return View(users);
        }

        // GET: RewardPoints/UserHistory/5
        public async Task<IActionResult> UserHistory(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var redeemHistory = await _context.RedeemPoints
                .Include(r => r.Order)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.RedeemDate)
                .ToListAsync();

            ViewBag.User = user;
            return View(redeemHistory);
        }

        // GET: RewardPoints/AdjustPoints/5
        public async Task<IActionResult> AdjustPoints(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.User = user;
            return View();
        }

        // POST: RewardPoints/AdjustPoints/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustPoints(int userId, int points, string action, string reason)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                if (action == "add")
                {
                    user.RewardPoints = (user.RewardPoints ?? 0) + points;
                    TempData["SuccessMessage"] = $"Đã cộng {points} điểm cho {user.CustomerName ?? user.Username}";
                }
                else if (action == "subtract")
                {
                    if ((user.RewardPoints ?? 0) < points)
                    {
                        TempData["ErrorMessage"] = "Số điểm hiện tại không đủ để trừ";
                        return RedirectToAction(nameof(AdjustPoints), new { userId });
                    }
                    user.RewardPoints -= points;
                    TempData["SuccessMessage"] = $"Đã trừ {points} điểm của {user.CustomerName ?? user.Username}";
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: RewardPoints/UserPoints/5
        [AllowAnonymous]
        public async Task<IActionResult> UserPoints(int userId)
        {
            var points = await _rewardPointService.GetUserRewardPoints(userId);
            return Json(new { points });
        }

        // POST: RewardPoints/UsePoints
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UsePoints(int userId, int points, int orderId)
        {
            try
            {
                var discountAmount = await _rewardPointService.CalculateDiscountAmount(points);
                var success = await _rewardPointService.UseRewardPoints(userId, points, orderId, discountAmount);
                
                if (success)
                {
                    // Cập nhật đơn hàng
                    var order = await _context.Orders.FindAsync(orderId);
                    if (order != null)
                    {
                        order.RewardPointsUsed = points;
                        order.DiscountAmount = discountAmount;
                        order.TotalAmount -= discountAmount;
                        await _context.SaveChangesAsync();
                    }

                    return Json(new { success = true, discountAmount = discountAmount });
                }
                else
                {
                    return Json(new { success = false, message = "Không đủ điểm hoặc có lỗi xảy ra" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
} 