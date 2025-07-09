using Microsoft.AspNetCore.Mvc;
using TinhLam.Data;
using TinhLam.ViewModels;
using TinhLam.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Globalization;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace TinhLam.Controllers
{
    public class PayPalOrderRequest
    {
        public bool UseRewardPoints { get; set; }
        public decimal DiscountAmount { get; set; }
    }

    public class CartController : Controller
    {
        private readonly TlinhContext db;
        private readonly PaypalClient _paypalClient;
        public CartController(TlinhContext context, PaypalClient paypalClient)
        {
            _paypalClient = paypalClient;
            db = context;
        }


        public List<CartItem> Cart
        {
            get
            {
                var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
                if (customerIdClaim != null) // Người dùng đã đăng nhập
                {
                    int customerId = int.Parse(customerIdClaim.Value);
                    return db.CartsUsers
                        .Where(c => c.UserId == customerId)
                        .Select(c => new CartItem
                        {
                            MaProduct = c.ProductId ?? 0,
                            TenProduct = c.Product.ProductName,
                            Price = c.UnitPrice,
                            Hinh = c.Product.Image ?? string.Empty,
                            SoLuong = c.Quantity,
                            ProductSizeId = c.ProductSizeId,
                            Size = c.ProductSize != null ? c.ProductSize.Size : ""
                        }).ToList();
                }
                else // Khách vãng lai
                {
                    return HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
                }
            }
        }

        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int productSizeId, int quantity = 1)
        {
            var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
            if (customerIdClaim != null) // Nếu người dùng đã đăng nhập
            {
                int customerId = int.Parse(customerIdClaim.Value);
                var existingItem = db.CartsUsers.SingleOrDefault(c => c.UserId == customerId && c.ProductId == id && c.ProductSizeId == productSizeId);
                if (existingItem == null)
                {
                    var product = db.Products.SingleOrDefault(p => p.ProductId == id);
                    if (product == null)
                    {
                        TempData["Message"] = $"Không tìm thấy sản phẩm có mã {id}";
                        return Redirect("/404");
                    }
                    var newCartItem = new CartsUser
                    {
                        UserId = customerId,
                        ProductId = product.ProductId,
                        ProductSizeId = productSizeId,
                        Quantity = quantity,
                        UnitPrice = product.Price,
                        TotalAmount = product.Price * quantity
                    };
                    db.CartsUsers.Add(newCartItem);
                }
                else
                {
                    existingItem.Quantity += quantity;
                    existingItem.TotalAmount = existingItem.Quantity * existingItem.UnitPrice;
                }
                db.SaveChanges();
            }
            else // Nếu là khách vãng lai
            {
                var gioHang = Cart;
                var item = gioHang.SingleOrDefault(p => p.MaProduct == id && p.ProductSizeId == productSizeId);
                if (item == null)
                {
                    var product = db.Products.SingleOrDefault(p => p.ProductId == id);
                    if (product == null)
                    {
                        TempData["Message"] = $"Không tìm thấy sản phẩm có mã {id}";
                        return Redirect("/404");
                    }
                    var productSize = db.ProductSizes.SingleOrDefault(ps => ps.ProductSizeId == productSizeId);
                    item = new CartItem
                    {
                        MaProduct = product.ProductId,
                        TenProduct = product.ProductName,
                        Price = Math.Round(product.Price, 0),
                        Hinh = product.Image ?? string.Empty,
                        SoLuong = quantity,
                        ProductSizeId = productSizeId,
                        Size = productSize?.Size ?? ""
                    };
                    gioHang.Add(item);
                }
                else
                {
                    item.SoLuong += quantity;
                }
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }
            return RedirectToAction("Index");
        }


        public IActionResult RemoveCart(int id, int productSizeId)
        {
            var customerIdClaim = HttpContext.User.Claims.FirstOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
            if (customerIdClaim != null) // Nếu user đã đăng nhập
            {
                int customerId = int.Parse(customerIdClaim.Value);

                var item = db.CartsUsers.FirstOrDefault(c => c.UserId == customerId && c.ProductId == id && c.ProductSizeId == productSizeId);

                if (item != null)
                {
                    db.CartsUsers.Remove(item);
                    db.SaveChanges();
                }
            }
            else // Nếu là khách vãng lai
            {
                var gioHang = Cart;
                var item = gioHang.FirstOrDefault(p => p.MaProduct == id && p.ProductSizeId == productSizeId);
                if (item != null)
                {
                    gioHang.Remove(item);
                    HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
                }
            }
            return RedirectToAction("Index");
        }




        public IActionResult SyncCartAfterLogin()
        {
            var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
            if (customerIdClaim == null) return RedirectToAction("Index");

            int customerId = int.Parse(customerIdClaim.Value);
            var sessionCart = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();

            foreach (var item in sessionCart)
            {
                var existingItem = db.CartsUsers.SingleOrDefault(c => c.UserId == customerId && c.ProductId == item.MaProduct && c.ProductSizeId == item.ProductSizeId);
                if (existingItem == null)
                {
                    db.CartsUsers.Add(new CartsUser
                    {
                        UserId = customerId,
                        ProductId = item.MaProduct,
                        ProductSizeId = item.ProductSizeId,
                        Quantity = item.SoLuong,
                        UnitPrice = item.Price,
                        TotalAmount = item.SoLuong * item.Price
                    });
                }
                else
                {
                    existingItem.Quantity += item.SoLuong;
                    existingItem.TotalAmount = existingItem.Quantity * existingItem.UnitPrice;
                }
            }

            db.SaveChanges();
            HttpContext.Session.Remove(MySetting.CART_KEY);
            return RedirectToAction("Index");
        }





        [HttpGet]
        public IActionResult ThanhToan()
        {
            
            if(Cart.Count == 0)
            {
                return Redirect("/Categories/Category");
            }
            ViewBag.PaypalClientId = _paypalClient.ClientId;
            return View(Cart);
        }

        [HttpGet]
        public IActionResult GetUserRewardPoints()
        {
            var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
            if (customerIdClaim != null)
            {
                int customerId = int.Parse(customerIdClaim.Value);
                var user = db.Users.SingleOrDefault(u => u.UserId == customerId);
                if (user != null)
                {
                    return Json(new { 
                        isLoggedIn = true, 
                        rewardPoints = user.RewardPoints ?? 0 
                    });
                }
            }
            return Json(new { isLoggedIn = false, rewardPoints = 0 });
        }

        [HttpPost]
        public IActionResult ThanhToan(DatHangVM model)
        {
            if (ModelState.IsValid)
            {
                var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
                int? customerId = customerIdClaim != null ? int.Parse(customerIdClaim.Value) : (int?)null;

                // Đảm bảo PhoneNumber không null
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "Số điện thoại là bắt buộc");
                    return View(Cart);
                }

                // Tính toán tổng tiền và giảm giá
                decimal subtotal = Cart.Sum(item => item.ThanhTien);
                decimal finalTotal = subtotal;

                // Xử lý giảm giá từ điểm thưởng
                if (model.UseRewardPoints && customerId.HasValue)
                {
                    var user = db.Users.SingleOrDefault(u => u.UserId == customerId.Value);
                    if (user != null && user.RewardPoints >= 50)
                    {
                        // Chỉ sử dụng 50 điểm mỗi lần (5% giảm giá)
                        model.DiscountAmount = Math.Floor(subtotal * 5 / 100); // 5% giảm giá
                        model.PointsUsed = 50; // Chỉ sử dụng 50 điểm
                        finalTotal = subtotal - model.DiscountAmount;

                        // Trừ điểm thưởng
                        user.RewardPoints -= model.PointsUsed;
                        db.SaveChanges();
                    }
                }

                var hoadon = new Order
                {
                    UserId = customerId,
                    CustomerName = model.CustomerName ?? "Khách hàng",
                    PhoneNumber = model.PhoneNumber,
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    PaymentMethod = "COD",
                    Status = "Pending",
                    Notes = model.Notes,
                    TotalAmount = finalTotal,
                    City = model.City,
                    District = model.District,
                    Ward = model.Ward,
                    StreetAddress = model.StreetAddress,
                    DiscountAmount = model.DiscountAmount,
                    RewardPointsUsed = model.PointsUsed
                };

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Add(hoadon);
                        db.SaveChanges();

                        var cthds = new List<OrderDetail>();
                        foreach (var item in Cart)
                        {
                            cthds.Add(new OrderDetail
                            {
                                OrderId = hoadon.OrderId,
                                Quantity = item.SoLuong,
                                UnitPrice = item.Price,
                                ProductId = item.MaProduct,
                                ProductSizeId = item.ProductSizeId
                            });

                            // Cập nhật số lượng tồn kho của ProductSize
                            if (item.ProductSizeId.HasValue)
                            {
                                var productSize = db.ProductSizes.SingleOrDefault(ps => ps.ProductSizeId == item.ProductSizeId.Value);
                                if (productSize != null)
                                {
                                    productSize.StockQuantity -= item.SoLuong;
                                    if (productSize.StockQuantity < 0) productSize.StockQuantity = 0;
                                }
                            }
                        }
                        db.AddRange(cthds);
                        db.SaveChanges();

                        if (customerId != null)
                        {
                            var cartItems = db.CartsUsers.Where(c => c.UserId == customerId).ToList();
                            db.CartsUsers.RemoveRange(cartItems);
                        }
                        HttpContext.Session.Remove(MySetting.CART_KEY);
                        db.SaveChanges();
                        transaction.Commit();

                        return RedirectToAction("PaymentSuccess", "Cart", new { orderId = hoadon.OrderId });
                    }
                    catch
                    {
                        transaction.Rollback();
                        ModelState.AddModelError("", "Có lỗi xảy ra khi đặt hàng. Vui lòng thử lại.");
                    }
                }
            }
            return View(Cart);
        }


        public IActionResult PaymentSuccess()
        {
            return View();
        }


        [HttpPost("/Cart/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder([FromBody] PayPalOrderRequest request, CancellationToken cancellationToken)
        {
            // Tính toán tổng tiền cuối cùng
            decimal subtotal = Cart.Sum(p => p.ThanhTien);
            decimal finalAmount = subtotal;

            // Xử lý giảm giá nếu có
            if (request.UseRewardPoints)
            {
                var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
                if (customerIdClaim != null)
                {
                    int customerId = int.Parse(customerIdClaim.Value);
                    var user = db.Users.SingleOrDefault(u => u.UserId == customerId);
                    if (user != null && user.RewardPoints >= 50)
                    {
                        decimal discountAmount = Math.Floor(subtotal * 5 / 100); // 5% giảm giá
                        finalAmount = subtotal - discountAmount;
                    }
                }
            }

            // Lấy tổng tiền cuối cùng và đảm bảo không có dấu phẩy hoặc chấm
            var tongTien = finalAmount.ToString("F2", CultureInfo.InvariantCulture);

            var donViTienTe = "USD";
            var maDonHangThamChieu = "DH" + DateTime.Now.Ticks.ToString();

            try
            {
                var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, maDonHangThamChieu);
                Console.WriteLine("Response from PayPal: " + JsonConvert.SerializeObject(response));
                return Ok(response);

            }
            catch (Exception ex)
            {
                var err = new { ex.GetBaseException().Message };
                return BadRequest(err);
            }
        }

        [HttpPost("/Cart/capture-paypal-order")]
        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);

                if (response.status == "COMPLETED")
                {
                    var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
                    int? customerId = customerIdClaim != null ? int.Parse(customerIdClaim.Value) : (int?)null;

                    // Tính toán tổng tiền và giảm giá
                    decimal subtotal = Cart.Sum(item => item.ThanhTien);
                    decimal finalTotal = subtotal;
                    decimal discountAmount = 0;
                    int pointsUsed = 0;

                    // Xử lý giảm giá từ điểm thưởng nếu có
                    if (customerId.HasValue)
                    {
                        var user = db.Users.SingleOrDefault(u => u.UserId == customerId.Value);
                        if (user != null && user.RewardPoints >= 50)
                        {
                            // Chỉ sử dụng 50 điểm mỗi lần (5% giảm giá)
                            discountAmount = Math.Floor(subtotal * 5 / 100); // 5% giảm giá
                            pointsUsed = 50; // Chỉ sử dụng 50 điểm
                            finalTotal = subtotal - discountAmount;

                            // Trừ điểm thưởng
                            user.RewardPoints -= pointsUsed;
                            db.SaveChanges();
                        }
                    }

                    var hoadon = new Order
                    {
                        UserId = customerId,
                        CustomerName = "Khách hàng PayPal",
                        PhoneNumber = "0123456789",
                        OrderDate = DateOnly.FromDateTime(DateTime.Now),
                        PaymentMethod = "PayPal",
                        Status = "Completed",
                        Notes = "Thanh toán qua PayPal",
                        TotalAmount = finalTotal,
                        City = null,
                        District = null,
                        Ward = null,
                        StreetAddress = null,
                        DiscountAmount = discountAmount,
                        RewardPointsUsed = pointsUsed
                    };

                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.Add(hoadon);
                            db.SaveChanges();

                            var cthds = new List<OrderDetail>();
                            foreach (var item in Cart)
                            {
                                cthds.Add(new OrderDetail
                                {
                                    OrderId = hoadon.OrderId,
                                    Quantity = item.SoLuong,
                                    UnitPrice = item.Price,
                                    ProductId = item.MaProduct,
                                    ProductSizeId = item.ProductSizeId
                                });

                                // Cập nhật số lượng tồn kho của ProductSize
                                if (item.ProductSizeId.HasValue)
                                {
                                    var productSize = db.ProductSizes.SingleOrDefault(ps => ps.ProductSizeId == item.ProductSizeId.Value);
                                    if (productSize != null)
                                    {
                                        productSize.StockQuantity -= item.SoLuong;
                                        if (productSize.StockQuantity < 0) productSize.StockQuantity = 0;
                                    }
                                }
                            }
                            db.AddRange(cthds);
                            db.SaveChanges();

                            if (customerId != null)
                            {
                                var cartItems = db.CartsUsers.Where(c => c.UserId == customerId).ToList();
                                db.CartsUsers.RemoveRange(cartItems);
                            }
                            HttpContext.Session.Remove(MySetting.CART_KEY);
                            db.SaveChanges();
                            transaction.Commit();

                            return RedirectToAction("PaymentSuccess", "Cart", new { orderId = hoadon.OrderId });
                        }
                        catch
                        {
                            transaction.Rollback();
                            ModelState.AddModelError("", "Có lỗi xảy ra khi đặt hàng. Vui lòng thử lại.");
                        }
                    }
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                var err = new { ex.GetBaseException().Message };
                return BadRequest(err);
            }
        }




    }
}


