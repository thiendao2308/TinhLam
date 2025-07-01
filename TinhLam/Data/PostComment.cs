using System;
using System.Collections.Generic;

namespace TinhLam.Data;

public partial class PostComment
{
    public int CommentId { get; set; }

    public int PostId { get; set; }

    public int? UserId { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string CommentContent { get; set; } = null!;

    public DateTime? CommentDate { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User? User { get; set; }
}
