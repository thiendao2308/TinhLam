using System.ComponentModel.DataAnnotations;

namespace TinhLam.ViewModels
{
    public class RegisterVM
    {
        
        [Required(ErrorMessage ="*")]
        public int UserId { get; set; }


        [Display(Name = "Tên khách hàng")]
        [Required(ErrorMessage = "*")]
        public string? CustomerName { get; set; }


        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "*")]
        [MaxLength(50, ErrorMessage = "Tên đăng nhập tối đa 50 kí tự")]
        public string Username { get; set; }


        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "*")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }


        
        


        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage ="Chưa đúng định dạng ")]
        [Required(ErrorMessage = "*")]
        public string Email { get; set; }


        [Display(Name = "Số điện thoại")]
        [MaxLength(10, ErrorMessage = "Số điện thoại tối đa 10 số")]
        [Required(ErrorMessage = "*")]
        public string PhoneNumber { get; set; }


        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "*")]
        public string? DiaChi { get; set; }
    }
}
