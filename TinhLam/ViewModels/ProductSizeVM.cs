using System.ComponentModel.DataAnnotations;

namespace TinhLam.ViewModels
{
    public class ProductSizeVM
    {
        [Display(Name = "ID Kích thước")]
        public int ProductSizeId { get; set; }

        [Display(Name = "ID Sản phẩm")]
        [Required(ErrorMessage = "ID sản phẩm là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "ID sản phẩm phải lớn hơn 0")]
        public int ProductId { get; set; }

        [Display(Name = "Kích thước")]
        [Required(ErrorMessage = "Kích thước là bắt buộc")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Kích thước phải từ 1-20 ký tự")]
        public string Size { get; set; } = string.Empty;

        [Display(Name = "Số lượng tồn kho")]
        [Required(ErrorMessage = "Số lượng tồn kho là bắt buộc")]
        [Range(0, 10000, ErrorMessage = "Số lượng tồn kho phải từ 0 đến 10000")]
        public int StockQuantity { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;
    }
} 