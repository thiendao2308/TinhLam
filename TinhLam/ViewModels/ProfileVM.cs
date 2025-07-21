using System.ComponentModel.DataAnnotations;
using TinhLam.Helpers;
using TinhLam.Data;

namespace TinhLam.ViewModels
{
    public class ProfileVM
    {
        [Display(Name = "ID")]
        public int UserId { get; set; }

        [Display(Name = "Tên khách hàng")]
        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [VietnameseName(ErrorMessage = "Tên khách hàng không hợp lệ")]
        [StringLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự")]
        public string? CustomerName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [VietnamesePhoneNumber(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        [VietnameseAddress(ErrorMessage = "Địa chỉ không hợp lệ")]
        public string? Diachi { get; set; }

        // Property cho tương thích với view cũ
        public string? DiaChi => Diachi;

        [Display(Name = "Điểm thưởng")]
        [Range(0, int.MaxValue, ErrorMessage = "Điểm thưởng không được âm")]
        public int? RewardPoints { get; set; }

        // Property cho tương thích với view cũ
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
