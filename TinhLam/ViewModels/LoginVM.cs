using System.ComponentModel.DataAnnotations;

namespace TinhLam.ViewModels
{
    public class LoginVM
    {
        [Display(Name="Tên đăng nhập")]
        [Required(ErrorMessage ="Chưa nhập tên đăng nhập")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Chưa nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
