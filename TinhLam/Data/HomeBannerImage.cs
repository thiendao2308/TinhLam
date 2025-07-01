using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TinhLam.Data;

public partial class HomeBannerImage
{
    public int ImageId { get; set; }

    
    [StringLength(500, ErrorMessage = "Đường dẫn ảnh không được vượt quá 500 ký tự")]
    public string ImageUrl { get; set; } = null!;

    [StringLength(255, ErrorMessage = "Tiêu đề không được vượt quá 255 ký tự")]
    public string? Caption { get; set; }

    [StringLength(500, ErrorMessage = "Liên kết không được vượt quá 500 ký tự")]
    [Url(ErrorMessage = "Liên kết không hợp lệ")]
    public string? Link { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Thứ tự hiển thị phải là số không âm")]
    public int? DisplayOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }
}
