using Microsoft.AspNetCore.Mvc;
using TinhLam.Data;
using TinhLam.ViewModels;
using TinhLam.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Globalization;
using Newtonsoft.Json;

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

       
        public List<CartItem> Cart => HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
        public IActionResult Index()
        {
            return View(Cart);
        }

        public IActionResult AddToCart(int id, int quantity = 1)
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
                    Price = Math.Round(product.Price, 0), // Làm tròn giá để loại bỏ dấu phẩy
                    Hinh = product.Image ?? string.Empty,
                    SizeOption = product.SizeOption,
                    SoLuong = quantity
                };
                gioHang.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }
            HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            return RedirectToAction("index");
        }
        public IActionResult RemoveCart(int id)
        {
            var gioHang = Cart;
            var item = gioHang.SingleOrDefault(p => p.MaProduct == id);
            if(item != null)
            {
                gioHang.Remove(item);
                HttpContext.Session.Set(MySetting.CART_KEY, gioHang);
            }
            return RedirectToAction("index");
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
                    ShippingAddress = model.ShippingAddress ?? khachHang.Email,
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
                        }
                        db.AddRange(cthds);
                        db.SaveChanges();

                        transaction.Commit(); // Chỉ commit nếu không có lỗi
                        HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>());

                        return View("Success"); // Điều hướng đến trang thành công
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
            return View("Success");
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
                        CustomerName = "Khách hàng PayPal", // Nếu có thông tin khách hàng, lấy từ response
                        ShippingAddress = "Địa chỉ PayPal",
                        PhoneNumber = "Số điện thoại PayPal",
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
                            }
                            db.AddRange(cthds);
                            db.SaveChanges();

                            transaction.Commit(); // Chỉ commit nếu không có lỗi
                            HttpContext.Session.Set<List<CartItem>>(MySetting.CART_KEY, new List<CartItem>()); // Xóa giỏ hàng sau khi thanh toán

                            return Ok(new { success = true, message = "Thanh toán thành công và đơn hàng đã được lưu vào database." });
                        }
                        catch
                        {
                            transaction.Rollback(); // Rollback nếu có lỗi
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
