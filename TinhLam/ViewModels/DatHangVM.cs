using System.ComponentModel.DataAnnotations;

namespace TinhLam.ViewModels
{
    public class DatHangVM
    {
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        public string? CustomerName { get; set; }
        
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }
        
        public string? Notes { get; set; }
        public decimal TotalAmount { get; set; }
        
        [Required(ErrorMessage = "Thành phố là bắt buộc")]
        public string? City { get; set; }
        
        [Required(ErrorMessage = "Quận/Huyện là bắt buộc")]
        public string? District { get; set; }
        
        [Required(ErrorMessage = "Phường/Xã là bắt buộc")]
        public string? Ward { get; set; }
        
        [Required(ErrorMessage = "Địa chỉ cụ thể là bắt buộc")]
        public string? StreetAddress { get; set; }

        // Thông tin điểm thưởng
        public bool UseRewardPoints { get; set; }
        public decimal DiscountAmount { get; set; }
        public int PointsUsed { get; set; }
    }
}
