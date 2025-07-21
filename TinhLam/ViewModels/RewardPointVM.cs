using System.ComponentModel.DataAnnotations;

namespace TinhLam.ViewModels
{
    public class RewardPointVM
    {
        [Display(Name = "ID Điểm thưởng")]
        public int RedeemPointId { get; set; }

        [Display(Name = "ID Khách hàng")]
        [Required(ErrorMessage = "ID khách hàng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "ID khách hàng phải lớn hơn 0")]
        public int UserId { get; set; }

        [Display(Name = "Số điểm")]
        [Required(ErrorMessage = "Số điểm là bắt buộc")]
        [Range(1, 100000, ErrorMessage = "Số điểm phải từ 1 đến 100000")]
        public int Points { get; set; }

        [Display(Name = "Loại giao dịch")]
        [Required(ErrorMessage = "Loại giao dịch là bắt buộc")]
        [StringLength(50, ErrorMessage = "Loại giao dịch không được vượt quá 50 ký tự")]
        public string TransactionType { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string? Description { get; set; }

        [Display(Name = "Ngày giao dịch")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Display(Name = "ID Đơn hàng")]
        [Range(0, int.MaxValue, ErrorMessage = "ID đơn hàng không được âm")]
        public int? OrderId { get; set; }
    }
} 