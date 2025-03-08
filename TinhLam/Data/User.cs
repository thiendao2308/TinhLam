using System;
using System.Collections.Generic;

namespace TinhLam.Data;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PhoneNumber { get; set; }

    public string Role { get; set; } = null!;

    public int? RewardPoints { get; set; }

    public string? RandomKey { get; set; }

    public bool HieuLuc { get; set; }

    public string? CustomerName { get; set; }

    public string? Diachi { get; set; }

    public string? Password { get; set; }

    public virtual ICollection<CartsUser> CartsUsers { get; set; } = new List<CartsUser>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<RedeemPoint> RedeemPoints { get; set; } = new List<RedeemPoint>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
