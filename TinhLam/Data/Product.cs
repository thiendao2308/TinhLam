using System;
using System.Collections.Generic;

namespace TinhLam.Data;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public string? SizeOption { get; set; }

    public int? CategoryId { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<CartsSession> CartsSessions { get; set; } = new List<CartsSession>();

    public virtual ICollection<CartsUser> CartsUsers { get; set; } = new List<CartsUser>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<RedeemPoint> RedeemPoints { get; set; } = new List<RedeemPoint>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
