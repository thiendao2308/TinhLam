using System.ComponentModel.DataAnnotations;

namespace TinhLam.ViewModels
{
    public class ChatMessageVM
    {
        [Display(Name = "ID Tin nhắn")]
        public int MessageId { get; set; }

        [Display(Name = "ID Người gửi")]
        [Required(ErrorMessage = "ID người gửi là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "ID người gửi phải lớn hơn 0")]
        public int SenderId { get; set; }

        [Display(Name = "ID Người nhận")]
        [Required(ErrorMessage = "ID người nhận là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "ID người nhận phải lớn hơn 0")]
        public int ReceiverId { get; set; }

        [Display(Name = "Nội dung tin nhắn")]
        [Required(ErrorMessage = "Nội dung tin nhắn là bắt buộc")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Nội dung tin nhắn phải từ 1-1000 ký tự")]
        public string Message { get; set; } = string.Empty;

        [Display(Name = "Thời gian gửi")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [Display(Name = "Đã đọc")]
        public bool IsRead { get; set; } = false;
    }
} 