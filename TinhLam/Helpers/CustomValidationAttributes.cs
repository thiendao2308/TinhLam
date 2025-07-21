using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TinhLam.Helpers
{
    public class VietnamesePhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult("Số điện thoại không được để trống");

            string phoneNumber = value.ToString()!;
            
            // Loại bỏ khoảng trắng và dấu gạch ngang
            phoneNumber = Regex.Replace(phoneNumber, @"[\s\-]", "");
            
            // Kiểm tra định dạng số điện thoại Việt Nam
            if (!Regex.IsMatch(phoneNumber, @"^(0|\+84)(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$"))
            {
                return new ValidationResult("Số điện thoại không hợp lệ. Vui lòng nhập đúng định dạng số điện thoại Việt Nam");
            }

            return ValidationResult.Success;
        }
    }

    public class VietnameseNameAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult("Tên không được để trống");

            string name = value.ToString()!;
            
            // Kiểm tra tên chỉ chứa chữ cái, dấu cách và dấu tiếng Việt
            if (!Regex.IsMatch(name, @"^[a-zA-ZÀ-ỹ\s]+$"))
            {
                return new ValidationResult("Tên chỉ được chứa chữ cái và dấu cách");
            }

            // Kiểm tra độ dài tối thiểu
            if (name.Trim().Length < 2)
            {
                return new ValidationResult("Tên phải có ít nhất 2 ký tự");
            }

            return ValidationResult.Success;
        }
    }

    public class StrongPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult("Mật khẩu không được để trống");

            string password = value.ToString()!;
            
            // Kiểm tra độ dài tối thiểu
            if (password.Length < 8)
            {
                return new ValidationResult("Mật khẩu phải có ít nhất 8 ký tự");
            }

            // Kiểm tra có ít nhất 1 chữ hoa
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                return new ValidationResult("Mật khẩu phải có ít nhất 1 chữ hoa");
            }

            // Kiểm tra có ít nhất 1 chữ thường
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                return new ValidationResult("Mật khẩu phải có ít nhất 1 chữ thường");
            }

            // Kiểm tra có ít nhất 1 số
            if (!Regex.IsMatch(password, @"\d"))
            {
                return new ValidationResult("Mật khẩu phải có ít nhất 1 số");
            }

            return ValidationResult.Success;
        }
    }

    public class VietnameseAddressAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult("Địa chỉ không được để trống");

            string address = value.ToString()!;
            
            // Kiểm tra độ dài tối thiểu
            if (address.Trim().Length < 10)
            {
                return new ValidationResult("Địa chỉ phải có ít nhất 10 ký tự");
            }

            // Kiểm tra độ dài tối đa
            if (address.Length > 200)
            {
                return new ValidationResult("Địa chỉ không được vượt quá 200 ký tự");
            }

            return ValidationResult.Success;
        }
    }

    public class PositiveNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Giá trị không được để trống");

            if (decimal.TryParse(value.ToString(), out decimal number))
            {
                if (number <= 0)
                {
                    return new ValidationResult("Giá trị phải lớn hơn 0");
                }
                return ValidationResult.Success;
            }

            return new ValidationResult("Giá trị phải là số hợp lệ");
        }
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success; // Cho phép null

            if (value is DateTime date)
            {
                if (date <= DateTime.Now)
                {
                    return new ValidationResult("Ngày phải là ngày trong tương lai");
                }
                return ValidationResult.Success;
            }

            return new ValidationResult("Giá trị phải là ngày hợp lệ");
        }
    }

    public class ImageFileAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly long _maxSizeInBytes = 5 * 1024 * 1024; // 5MB

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success; // Cho phép null

            if (value is IFormFile file)
            {
                // Kiểm tra extension
                string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ValidationResult($"Chỉ chấp nhận file ảnh: {string.Join(", ", _allowedExtensions)}");
                }

                // Kiểm tra kích thước file
                if (file.Length > _maxSizeInBytes)
                {
                    return new ValidationResult($"Kích thước file không được vượt quá {_maxSizeInBytes / (1024 * 1024)}MB");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Giá trị phải là file hợp lệ");
        }
    }
} 