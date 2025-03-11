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
    public class CartController : Controller
    {
        private readonly TLinhContext db;
        private readonly PaypalClient _paypalClient;
        public CartController(TLinhContext context, PaypalClient paypalClient)
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
                            MaProduct = c.ProductId ?? 0, // Tránh null
                            TenProduct = c.Product.ProductName,
                            Price = c.UnitPrice,
                            Hinh = c.Product.Image ?? string.Empty,
                            SoLuong = c.Quantity
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

        public IActionResult AddToCart(int id, int quantity = 1)
        {
            var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
            if (customerIdClaim != null) // Nếu người dùng đã đăng nhập
            {
                int customerId = int.Parse(customerIdClaim.Value);
                var existingItem = db.CartsUsers.SingleOrDefault(c => c.UserId == customerId && c.ProductId == id);
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
                var item = gioHang.SingleOrDefault(p => p.MaProduct == id);
                if (item == null)
                {
                    var product = db.Products.SingleOrDefault(p => p.ProductId == id);
                    if (product == null)
                    {
                        TempData["Message"] = $"Không tìm thấy sản phẩm có mã {id}";
                        return Redirect("/404");
                    }
                    item = new CartItem
                    {
                        MaProduct = product.ProductId,
                        TenProduct = product.ProductName,
                        Price = Math.Round(product.Price, 0),
                        Hinh = product.Image ?? string.Empty,
                        SoLuong = quantity
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


        public IActionResult RemoveCart(int id)
        {
            var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
            if (customerIdClaim != null) // Nếu user đã đăng nhập
            {
                int customerId = int.Parse(customerIdClaim.Value);
                var item = db.CartsUsers.SingleOrDefault(c => c.UserId == customerId && c.ProductId == id);
                if (item != null)
                {
                    db.CartsUsers.Remove(item);
                    db.SaveChanges();
                }
            }
            else // Nếu là khách vãng lai
            {
                var gioHang = Cart;
                var item = gioHang.SingleOrDefault(p => p.MaProduct == id);
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
                var existingItem = db.CartsUsers.SingleOrDefault(c => c.UserId == customerId && c.ProductId == item.MaProduct);
                if (existingItem == null)
                {
                    db.CartsUsers.Add(new CartsUser
                    {
                        UserId = customerId,
                        ProductId = item.MaProduct,
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

        [HttpPost]
        public IActionResult ThanhToan(DatHangVM model)
        {
            if (ModelState.IsValid)
            {
                var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
                int? customerId = customerIdClaim != null ? int.Parse(customerIdClaim.Value) : (int?)null;

                var khachHang = new User();
                if (model.GiongKhachHang)
                {
                    khachHang = db.Users.SingleOrDefault(kh => kh.UserId == customerId);
                }

                var hoadon = new Order
                {
                    UserId = customerId,
                    CustomerName = model.CustomerName ?? khachHang.Username,
                    ShippingAddress = model.ShippingAddress ?? khachHang.Diachi,
                    PhoneNumber = model.PhoneNumber ?? khachHang.PhoneNumber,
                    OrderDate = DateOnly.FromDateTime(DateTime.Now),
                    PaymentMethod = "COD",
                    Status = "Pending",
                    Notes = model.Notes,
                    TotalAmount = Cart.Sum(item => item.ThanhTien),
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
                            });

                            // 🔴 Cập nhật số lượng tồn kho
                            var product = db.Products.SingleOrDefault(p => p.ProductId == item.MaProduct);
                            if (product != null)
                            {
                                product.StockQuantity -= item.SoLuong;
                                if (product.StockQuantity < 0) product.StockQuantity = 0; // Đảm bảo không âm
                            }
                        }
                        db.AddRange(cthds);
                        db.SaveChanges();

                        // **XÓA GIỎ HÀNG SAU KHI ĐẶT HÀNG**
                        if (customerId != null)
                        {
                            var cartItems = db.CartsUsers.Where(c => c.UserId == customerId).ToList();
                            db.CartsUsers.RemoveRange(cartItems);
                        }
                        HttpContext.Session.Remove(MySetting.CART_KEY); // Xóa giỏ hàng session
                        db.SaveChanges();
                        transaction.Commit(); // Commit nếu không có lỗi

                        return RedirectToAction("PaymentSuccess", "Cart", new { orderId = hoadon.OrderId });
                    }
                    catch
                    {
                        transaction.Rollback(); // Rollback nếu có lỗi
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
        public async Task<IActionResult> CreatePaypalOrder(CancellationToken cancellationToken)
        {
            // Lấy tổng tiền và đảm bảo không có dấu phẩy hoặc chấm
            var tongTien = Cart.Sum(p => p.ThanhTien).ToString("F2", CultureInfo.InvariantCulture);

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
                // Gửi yêu cầu bắt thanh toán từ PayPal
                var response = await _paypalClient.CaptureOrder(orderID);

                // Kiểm tra nếu thanh toán thành công
                if (response.status == "COMPLETED")
                {
                    var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
                    int? customerId = customerIdClaim != null ? int.Parse(customerIdClaim.Value) : (int?)null;

                    var hoadon = new Order
                    {
                        UserId = customerId,
                        CustomerName = "đào đức thuận", // Nếu có thông tin khách hàng, lấy từ response
                        ShippingAddress = "34 Nguyễn Văn Cừ ",
                        PhoneNumber = "0123456789",
                        OrderDate = DateOnly.FromDateTime(DateTime.Now),
                        PaymentMethod = "PayPal",
                        Status = "Completed",
                        Notes = "Thanh toán qua PayPal",
                        TotalAmount = Cart.Sum(item => item.ThanhTien),
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
                                });

                                // 🔴 Cập nhật số lượng tồn kho
                                var product = db.Products.SingleOrDefault(p => p.ProductId == item.MaProduct);
                                if (product != null)
                                {
                                    product.StockQuantity -= item.SoLuong;
                                    if (product.StockQuantity < 0) product.StockQuantity = 0; // Đảm bảo không âm
                                }
                            }
                            db.AddRange(cthds);
                            db.SaveChanges();

                            // **XÓA GIỎ HÀNG SAU KHI THANH TOÁN**
                            if (customerId != null)
                            {
                                var cartItems = db.CartsUsers.Where(c => c.UserId == customerId).ToList();
                                db.CartsUsers.RemoveRange(cartItems);
                            }
                            HttpContext.Session.Remove(MySetting.CART_KEY); // Xóa giỏ hàng session
                            db.SaveChanges();
                            transaction.Commit();

                            return RedirectToAction("PaymentSuccess", "Cart", new { orderId = hoadon.OrderId });
                        }
                        catch
                        {
                            transaction.Rollback();
                            return BadRequest(new { success = false, message = "Có lỗi xảy ra khi lưu đơn hàng vào database." });
                        }
                    }

                }
                else
                {
                    return BadRequest(new { success = false, message = "Thanh toán PayPal không thành công." });
                }
            }
            catch (Exception ex)
            {
                var err = new { ex.GetBaseException().Message };
                return BadRequest(err);
            }
        }




    }
}
