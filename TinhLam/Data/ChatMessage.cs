using System;
using System.Collections.Generic;

namespace TinhLam.Data;

public partial class ChatMessage
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime? Timestamp { get; set; }

    public virtual User User { get; set; } = null!;
}
