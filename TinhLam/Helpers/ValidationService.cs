using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TinhLam.Helpers
{
    public static class ValidationService
    {
        public static List<string> GetValidationErrors(object model)
        {
            var errors = new List<string>();
            var validationContext = new ValidationContext(model);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, validationContext, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    errors.Add(validationResult.ErrorMessage ?? "Lỗi validation không xác định");
                }
            }

            return errors;
        }

        public static bool IsValid(object model)
        {
            var validationContext = new ValidationContext(model);
            return Validator.TryValidateObject(model, validationContext, null, true);
        }

        public static List<string> GetModelStateErrors(ModelStateDictionary modelState)
        {
            var errors = new List<string>();
            foreach (var state in modelState.Values)
            {
                foreach (var error in state.Errors)
                {
                    errors.Add(error.ErrorMessage);
                }
            }
            return errors;
        }

        public static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool ValidateVietnamesePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Loại bỏ khoảng trắng và dấu gạch ngang
            phoneNumber = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[\s\-]", "");

            // Kiểm tra định dạng số điện thoại Việt Nam
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, 
                @"^(0|\+84)(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$");
        }

        public static bool ValidateStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            // Kiểm tra có ít nhất 1 chữ hoa
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[A-Z]"))
                return false;

            // Kiểm tra có ít nhất 1 chữ thường
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"[a-z]"))
                return false;

            // Kiểm tra có ít nhất 1 số
            if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"\d"))
                return false;

            return true;
        }

        public static bool ValidateVietnameseName(string name)
        {
            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 2)
                return false;

            // Kiểm tra tên chỉ chứa chữ cái, dấu cách và dấu tiếng Việt
            return System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-ZÀ-ỹ\s]+$");
        }

        public static bool ValidateVietnameseAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return false;

            // Kiểm tra độ dài tối thiểu và tối đa
            return address.Trim().Length >= 10 && address.Length <= 200;
        }

        public static bool ValidatePositiveNumber(decimal number)
        {
            return number > 0;
        }

        public static bool ValidateImageFile(IFormFile file)
        {
            if (file == null)
                return true; // Cho phép null

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var maxSizeInBytes = 5 * 1024 * 1024; // 5MB

            // Kiểm tra extension
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return false;

            // Kiểm tra kích thước file
            if (file.Length > maxSizeInBytes)
                return false;

            return true;
        }
    }
} 