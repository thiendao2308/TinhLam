using System;
using System.Collections.Generic;

namespace TinhLam.Data;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public DateOnly OrderDate { get; set; }

    public TimeOnly OrderTime { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string? Notes { get; set; }

    public string? City { get; set; }

    public string? District { get; set; }

    public string? Ward { get; set; }

    public string? StreetAddress { get; set; }

    public int? RewardPointsUsed { get; set; }

    public decimal? DiscountAmount { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<RedeemPoint> RedeemPoints { get; set; } = new List<RedeemPoint>();

    public virtual User? User { get; set; }
}
