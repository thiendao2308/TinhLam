using System.ComponentModel.DataAnnotations;
using TinhLam.Helpers;

namespace TinhLam.ViewModels
{
    public class DatHangVM
    {
        [Display(Name = "Tên khách hàng")]
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [VietnameseName(ErrorMessage = "Tên khách hàng không hợp lệ")]
        [StringLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự")]
        public string? CustomerName { get; set; }
        
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [VietnamesePhoneNumber(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }
        
        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }
        
        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }
        
        [Display(Name = "Thành phố")]
        [Required(ErrorMessage = "Thành phố là bắt buộc")]
        [StringLength(50, ErrorMessage = "Thành phố không được vượt quá 50 ký tự")]
        public string? City { get; set; }
        
        [Display(Name = "Quận/Huyện")]
        [Required(ErrorMessage = "Quận/Huyện là bắt buộc")]
        [StringLength(50, ErrorMessage = "Quận/Huyện không được vượt quá 50 ký tự")]
        public string? District { get; set; }
        
        [Display(Name = "Phường/Xã")]
        [Required(ErrorMessage = "Phường/Xã là bắt buộc")]
        [StringLength(50, ErrorMessage = "Phường/Xã không được vượt quá 50 ký tự")]
        public string? Ward { get; set; }
        
        [Display(Name = "Địa chỉ cụ thể")]
        [Required(ErrorMessage = "Địa chỉ cụ thể là bắt buộc")]
        [VietnameseAddress(ErrorMessage = "Địa chỉ cụ thể không hợp lệ")]
        public string? StreetAddress { get; set; }

        // Thông tin điểm thưởng
        [Display(Name = "Sử dụng điểm thưởng")]
        public bool UseRewardPoints { get; set; }
        
        [Display(Name = "Số tiền giảm")]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền giảm không được âm")]
        public decimal DiscountAmount { get; set; }
        
        [Display(Name = "Số điểm sử dụng")]
        [Range(0, int.MaxValue, ErrorMessage = "Số điểm sử dụng không được âm")]
        public int PointsUsed { get; set; }
    }
}
