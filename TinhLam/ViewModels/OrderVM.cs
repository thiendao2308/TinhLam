using System.ComponentModel.DataAnnotations;
using TinhLam.Helpers;

namespace TinhLam.ViewModels
{
    public class OrderVM
    {
        [Display(Name = "ID Đơn hàng")]
        public int OrderId { get; set; }

        [Display(Name = "ID Khách hàng")]
        public int? UserId { get; set; }

        [Display(Name = "Tên khách hàng")]
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [VietnameseName(ErrorMessage = "Tên khách hàng không hợp lệ")]
        [StringLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự")]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [VietnamesePhoneNumber(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Display(Name = "Ngày đặt hàng")]
        [Required(ErrorMessage = "Ngày đặt hàng là bắt buộc")]
        [DataType(DataType.Date)]
        public DateOnly OrderDate { get; set; }

        [Display(Name = "Giờ đặt hàng")]
        [Required(ErrorMessage = "Giờ đặt hàng là bắt buộc")]
        [DataType(DataType.Time)]
        public TimeOnly OrderTime { get; set; }

        [Display(Name = "Tổng tiền")]
        [Required(ErrorMessage = "Tổng tiền là bắt buộc")]
        [PositiveNumber(ErrorMessage = "Tổng tiền phải lớn hơn 0")]
        [Range(0.01, 999999999, ErrorMessage = "Tổng tiền phải từ 0.01 đến 999,999,999")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Trạng thái")]
        [Required(ErrorMessage = "Trạng thái là bắt buộc")]
        [StringLength(50, ErrorMessage = "Trạng thái không được vượt quá 50 ký tự")]
        public string? Status { get; set; }

        [Display(Name = "Phương thức thanh toán")]
        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
        [StringLength(50, ErrorMessage = "Phương thức thanh toán không được vượt quá 50 ký tự")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Display(Name = "Ghi chú")]
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        public string? Notes { get; set; }

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

        [Display(Name = "Điểm thưởng sử dụng")]
        [Range(0, int.MaxValue, ErrorMessage = "Điểm thưởng sử dụng không được âm")]
        public int? RewardPointsUsed { get; set; }

        [Display(Name = "Số tiền giảm")]
        [Range(0, double.MaxValue, ErrorMessage = "Số tiền giảm không được âm")]
        public decimal? DiscountAmount { get; set; }
    }
} 