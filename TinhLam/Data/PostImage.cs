using System;
using System.Collections.Generic;

namespace TinhLam.Data;

public partial class PostImage
{
    public int ImageId { get; set; }

    public int PostId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? Caption { get; set; }

    public int? DisplayOrder { get; set; }

    public virtual Post Post { get; set; } = null!;
}
