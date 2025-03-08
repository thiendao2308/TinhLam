using System;
using System.Collections.Generic;

namespace TinhLam.Data;

public partial class CartsSession
{
    public int CartSessionId { get; set; }

    public Guid? SessionId { get; set; }

    public decimal TotalAmount { get; set; }

    public int? ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Product? Product { get; set; }
}
