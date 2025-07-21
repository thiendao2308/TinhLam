using System.ComponentModel.DataAnnotations;
using TinhLam.Helpers;

namespace TinhLam.ViewModels
{
    public class ProductVM
    {
        [Display(Name = "ID Sản phẩm")]
        public int ProductId { get; set; }

        // Properties cho tương thích với view cũ
        public int MaProduct => ProductId;
        public string TenProduct => ProductName;
        public string Hinh => Image;
        public string MoTaNgan => Description ?? "";

        [Display(Name = "Tên sản phẩm")]
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Tên sản phẩm phải từ 3-200 ký tự")]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "Giá")]
        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [PositiveNumber(ErrorMessage = "Giá sản phẩm phải lớn hơn 0")]
        [Range(0.01, 999999999, ErrorMessage = "Giá sản phẩm phải từ 0.01 đến 999,999,999")]
        public decimal Price { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự")]
        public string? Description { get; set; }

        [Display(Name = "Hình ảnh")]
        public string? Image { get; set; }

        [Display(Name = "File ảnh")]
        [ImageFile(ErrorMessage = "File ảnh không hợp lệ")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Danh mục")]
        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục")]
        public int? CategoryId { get; set; }

        [Display(Name = "Trạng thái")]
        public bool? IsActive { get; set; } = true;
    }
}
