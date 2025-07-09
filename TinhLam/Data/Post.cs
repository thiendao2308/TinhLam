using System;
using System.Collections.Generic;

namespace TinhLam.Data;

public partial class Post
{
    public int PostId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string? Image { get; set; }

    public string? Author { get; set; }

    public DateTime CreatedAt { get; set; }

    public string PostedBy { get; set; } = null!;

    public bool IsPublished { get; set; }

    public string? Tags { get; set; }

    public int ViewCount { get; set; }

    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    public virtual ICollection<PostImage> PostImages { get; set; } = new List<PostImage>();
}
