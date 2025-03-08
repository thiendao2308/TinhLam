using System;
using System.Collections.Generic;

namespace TinhLam.Data;

public partial class CartsUser
{
    public int CartUserId { get; set; }

    public int? UserId { get; set; }

    public decimal TotalAmount { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
