using System.ComponentModel.DataAnnotations;
using TinhLam.Helpers;

namespace TinhLam.ViewModels
{
    public class HomeBannerVM
    {
        [Display(Name = "ID Banner")]
        public int BannerId { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Tiêu đề phải từ 3-200 ký tự")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [Display(Name = "Hình ảnh")]
        [Required(ErrorMessage = "Hình ảnh là bắt buộc")]
        [ImageFile(ErrorMessage = "File ảnh không hợp lệ")]
        public IFormFile ImageFile { get; set; } = null!;

        [Display(Name = "Liên kết")]
        [StringLength(500, ErrorMessage = "Liên kết không được vượt quá 500 ký tự")]
        [Url(ErrorMessage = "Liên kết không đúng định dạng URL")]
        public string? Link { get; set; }

        [Display(Name = "Thứ tự hiển thị")]
        [Required(ErrorMessage = "Thứ tự hiển thị là bắt buộc")]
        [Range(1, 100, ErrorMessage = "Thứ tự hiển thị phải từ 1 đến 100")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
    }
} 