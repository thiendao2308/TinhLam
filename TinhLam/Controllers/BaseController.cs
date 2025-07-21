using Microsoft.AspNetCore.Mvc;
using TinhLam.Helpers;

namespace TinhLam.Controllers
{
    public abstract class BaseController : Controller
    {
        protected bool ValidateModel(object model)
        {
            if (!ModelState.IsValid)
            {
                return false;
            }

            var validationErrors = ValidationService.GetValidationErrors(model);
            if (validationErrors.Any())
            {
                foreach (var error in validationErrors)
                {
                    ModelState.AddModelError("", error);
                }
                return false;
            }

            return true;
        }

        protected JsonResult ValidationErrorResponse()
        {
            var errors = ValidationService.GetModelStateErrors(ModelState);
            return Json(new { success = false, errors = errors });
        }

        protected JsonResult SuccessResponse(object? data = null, string message = "Thao tác thành công")
        {
            return Json(new { success = true, message = message, data = data });
        }

        protected JsonResult ErrorResponse(string message = "Có lỗi xảy ra")
        {
            return Json(new { success = false, message = message });
        }

        protected void AddModelError(string key, string message)
        {
            ModelState.AddModelError(key, message);
        }

        protected void AddModelError(string message)
        {
            ModelState.AddModelError("", message);
        }

        protected bool IsValidEmail(string email)
        {
            return ValidationService.ValidateEmail(email);
        }

        protected bool IsValidVietnamesePhoneNumber(string phoneNumber)
        {
            return ValidationService.ValidateVietnamesePhoneNumber(phoneNumber);
        }

        protected bool IsValidStrongPassword(string password)
        {
            return ValidationService.ValidateStrongPassword(password);
        }

        protected bool IsValidVietnameseName(string name)
        {
            return ValidationService.ValidateVietnameseName(name);
        }

        protected bool IsValidVietnameseAddress(string address)
        {
            return ValidationService.ValidateVietnameseAddress(address);
        }

        protected bool IsValidPositiveNumber(decimal number)
        {
            return ValidationService.ValidatePositiveNumber(number);
        }

        protected bool IsValidImageFile(IFormFile file)
        {
            return ValidationService.ValidateImageFile(file);
        }
    }
} 