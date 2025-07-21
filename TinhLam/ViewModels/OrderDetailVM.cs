using System.ComponentModel.DataAnnotations;
using TinhLam.Helpers;

namespace TinhLam.ViewModels
{
    public class OrderDetailVM
    {
        [Display(Name = "ID Chi tiết đơn hàng")]
        public int OrderDetailId { get; set; }

        [Display(Name = "ID Đơn hàng")]
        [Required(ErrorMessage = "ID đơn hàng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "ID đơn hàng phải lớn hơn 0")]
        public int OrderId { get; set; }

        [Display(Name = "ID Sản phẩm")]
        [Required(ErrorMessage = "ID sản phẩm là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "ID sản phẩm phải lớn hơn 0")]
        public int ProductId { get; set; }

        [Display(Name = "Số lượng")]
        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, 1000, ErrorMessage = "Số lượng phải từ 1 đến 1000")]
        public int Quantity { get; set; }

        [Display(Name = "Đơn giá")]
        [Required(ErrorMessage = "Đơn giá là bắt buộc")]
        [PositiveNumber(ErrorMessage = "Đơn giá phải lớn hơn 0")]
        [Range(0.01, 999999999, ErrorMessage = "Đơn giá phải từ 0.01 đến 999,999,999")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Thành tiền")]
        [Required(ErrorMessage = "Thành tiền là bắt buộc")]
        [PositiveNumber(ErrorMessage = "Thành tiền phải lớn hơn 0")]
        [Range(0.01, 999999999, ErrorMessage = "Thành tiền phải từ 0.01 đến 999,999,999")]
        public decimal Subtotal { get; set; }

        [Display(Name = "Kích thước")]
        [StringLength(20, ErrorMessage = "Kích thước không được vượt quá 20 ký tự")]
        public string? Size { get; set; }
    }
} 