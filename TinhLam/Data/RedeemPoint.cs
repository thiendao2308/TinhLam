using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TinhLam.Data;

public partial class RedeemPoint
{
    public int RedeemId { get; set; }

    public int UserId { get; set; }

    public int OrderId { get; set; }

    public int PointsUsed { get; set; }

    public decimal DiscountAmount { get; set; }

    public DateTime RedeemDate { get; set; }

    public string? Description { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
