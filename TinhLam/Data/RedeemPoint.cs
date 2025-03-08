using System;
using System.Collections.Generic;

namespace TinhLam.Data;

public partial class RedeemPoint
{
    public int RedeemId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int PointsUsed { get; set; }

    public DateTime? RedeemDate { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
