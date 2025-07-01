using Microsoft.AspNetCore.Mvc;
using TinhLam.Helpers;
using TinhLam.ViewModels;
using TinhLam.Data;

namespace TinhLam.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly TlinhContext db;

        public CartViewComponent(TlinhContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var customerIdClaim = HttpContext.User.Claims.SingleOrDefault(p => p.Type == MySetting.CLAIM_CUSTOMERID);
            List<CartItem> cart;

            if (customerIdClaim != null) // Nếu đã đăng nhập
            {
                int customerId = int.Parse(customerIdClaim.Value);
                cart = db.CartsUsers
                    .Where(c => c.UserId == customerId)
                    .Select(c => new CartItem
                    {
                        MaProduct = c.ProductId ?? 0,
                        TenProduct = c.Product.ProductName,
                        Price = c.UnitPrice,
                        Hinh = c.Product.Image ?? string.Empty,
                        SoLuong = c.Quantity
                    }).ToList();
            }
            else // Nếu là khách vãng lai
            {
                cart = HttpContext.Session.Get<List<CartItem>>(MySetting.CART_KEY) ?? new List<CartItem>();
            }

            return View("CartPanel", new CartModel
            {
                Quantity = cart.Sum(p => p.SoLuong),
                Total = cart.Sum(p => p.ThanhTien)
            });
        }
    }
}
