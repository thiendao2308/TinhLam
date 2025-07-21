using System.ComponentModel.DataAnnotations;
using TinhLam.Helpers;

namespace TinhLam.ViewModels
{
    public class PostVM
    {
        [Display(Name = "ID Bài viết")]
        public int PostId { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Tiêu đề phải từ 5-200 ký tự")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Nội dung")]
        [Required(ErrorMessage = "Nội dung là bắt buộc")]
        [StringLength(10000, MinimumLength = 10, ErrorMessage = "Nội dung phải từ 10-10000 ký tự")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Tóm tắt")]
        [StringLength(500, ErrorMessage = "Tóm tắt không được vượt quá 500 ký tự")]
        public string? Summary { get; set; }

        [Display(Name = "Hình ảnh chính")]
        [ImageFile(ErrorMessage = "File ảnh không hợp lệ")]
        public IFormFile? MainImageFile { get; set; }

        [Display(Name = "Hình ảnh bổ sung")]
        public List<IFormFile>? AdditionalImages { get; set; }

        [Display(Name = "Tác giả")]
        [Required(ErrorMessage = "Tác giả là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên tác giả không được vượt quá 100 ký tự")]
        public string Author { get; set; } = string.Empty;

        [Display(Name = "Trạng thái")]
        public bool IsPublished { get; set; } = false;

        [Display(Name = "Ngày xuất bản")]
        [FutureDate(ErrorMessage = "Ngày xuất bản phải là ngày trong tương lai")]
        public DateTime? PublishDate { get; set; }
    }
} 