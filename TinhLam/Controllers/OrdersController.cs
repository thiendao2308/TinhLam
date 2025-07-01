using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TinhLam.Data;
using Microsoft.AspNetCore.Authorization;
using TinhLam.Helpers;

namespace TinhLam.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly TlinhContext _context;
        private readonly IRewardPointService _rewardPointService;

        public OrdersController(TlinhContext context, IRewardPointService rewardPointService)
        {
            _context = context;
            _rewardPointService = rewardPointService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound("Không tìm thấy đơn hàng.");
            }

            return PartialView("_OrderDetailsPartial", order);
        }

        // GET: Orders
        public async Task<IActionResult> Index(string status, DateTime? startDate, DateTime? endDate)
        {
            var ordersQuery = _context.Orders.AsQueryable();

            // Lọc theo trạng thái đơn hàng nếu có
            if (!string.IsNullOrEmpty(status))
            {
                ordersQuery = ordersQuery.Where(o => o.Status == status);
            }

            // Chuyển đổi DateTime? sang DateOnly trước khi lọc
            if (startDate.HasValue)
            {
                DateOnly startDateOnly = DateOnly.FromDateTime(startDate.Value);
                ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDateOnly);
            }

            if (endDate.HasValue)
            {
                DateOnly endDateOnly = DateOnly.FromDateTime(endDate.Value);
                ordersQuery = ordersQuery.Where(o => o.OrderDate <= endDateOnly);
            }

            // Sắp xếp theo ngày đặt hàng và giờ đặt hàng giảm dần
            ordersQuery = ordersQuery.OrderByDescending(o => o.OrderDate)
                                     .ThenByDescending(o => o.OrderTime);

            // Thực thi truy vấn trước khi gọi await
            var orders = await ordersQuery.ToListAsync();

            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            var oldStatus = order.Status;
            order.Status = status;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            // Nếu đơn hàng chuyển sang trạng thái "Completed" và có UserId
            if (status == "Completed" && order.UserId.HasValue && oldStatus != "Completed")
            {
                // Tính điểm tích lũy
                var earnedPoints = await _rewardPointService.CalculateEarnedPoints(order.TotalAmount);
                if (earnedPoints > 0)
                {
                    await _rewardPointService.AddRewardPointsToUser(order.UserId.Value, earnedPoints);
                    TempData["SuccessMessage"] = $"Đã cộng {earnedPoints} điểm tích lũy cho khách hàng!";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        public IActionResult GetOrdersByPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return Content("<p class='error-message'>Số điện thoại không hợp lệ!</p>");
            }

            var orders = _context.Orders
                .Where(o => o.PhoneNumber == phone)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.OrderId,
                    o.OrderDate,
                    o.TotalAmount,
                    o.Status
                })
                .ToList();

            if (orders.Count == 0)
            {
                return Content("<p class='error-message'>Không tìm thấy đơn hàng nào!</p>");
            }

            var resultHtml = "<table class='order-table'>";
            resultHtml += "<tr><th>Mã đơn hàng</th><th>Ngày đặt</th><th>Tổng tiền</th><th>Trạng thái</th></tr>";

            foreach (var order in orders)
            {
                resultHtml += $"<tr><td>{order.OrderId}</td><td>{order.OrderDate.ToShortDateString()}</td><td>{order.TotalAmount} VNĐ</td><td>{order.Status}</td></tr>";
            }

            resultHtml += "</table>";

            return Content(resultHtml, "text/html");
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product) // Load thông tin sản phẩm
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,UserId,CustomerName,PhoneNumber,ShippingAddress,OrderDate,OrderTime,TotalAmount,Status,PaymentMethod,Notes")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,UserId,CustomerName,PhoneNumber,ShippingAddress,OrderDate,OrderTime,TotalAmount,Status,PaymentMethod,Notes")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
