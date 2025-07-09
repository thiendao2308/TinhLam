using System;
using System.Collections.Generic;

namespace TinhLam.Data;

public partial class ProductSize
{
    public int ProductSizeId { get; set; }

    public int ProductId { get; set; }

    public string Size { get; set; } = null!;

    public int StockQuantity { get; set; }

    public virtual ICollection<CartsSession> CartsSessions { get; set; } = new List<CartsSession>();

    public virtual ICollection<CartsUser> CartsUsers { get; set; } = new List<CartsUser>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Product Product { get; set; } = null!;
}
