// Validation helper functions for client-side validation
const ValidationHelper = {
  // Email validation
  isValidEmail: function (email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  },

  // Vietnamese phone number validation
  isValidVietnamesePhoneNumber: function (phoneNumber) {
    if (!phoneNumber) return false;

    // Remove spaces and dashes
    phoneNumber = phoneNumber.replace(/[\s\-]/g, "");

    // Vietnamese phone number pattern
    const phoneRegex =
      /^(0|\+84)(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$/;
    return phoneRegex.test(phoneNumber);
  },

  // Strong password validation
  isValidStrongPassword: function (password) {
    if (!password || password.length < 8) return false;

    // Check for at least one uppercase letter
    if (!/[A-Z]/.test(password)) return false;

    // Check for at least one lowercase letter
    if (!/[a-z]/.test(password)) return false;

    // Check for at least one number
    if (!/\d/.test(password)) return false;

    return true;
  },

  // Vietnamese name validation
  isValidVietnameseName: function (name) {
    if (!name || name.trim().length < 2) return false;

    // Check for Vietnamese characters, letters, and spaces only
    const nameRegex = /^[a-zA-ZÀ-ỹ\s]+$/;
    return nameRegex.test(name);
  },

  // Vietnamese address validation
  isValidVietnameseAddress: function (address) {
    if (!address) return false;

    return address.trim().length >= 10 && address.length <= 200;
  },

  // Positive number validation
  isValidPositiveNumber: function (number) {
    const num = parseFloat(number);
    return !isNaN(num) && num > 0;
  },

  // File validation
  isValidImageFile: function (file) {
    if (!file) return true; // Allow null

    const allowedExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    const maxSizeInBytes = 5 * 1024 * 1024; // 5MB

    // Check extension
    const extension = file.name
      .toLowerCase()
      .substring(file.name.lastIndexOf("."));
    if (!allowedExtensions.includes(extension)) {
      return false;
    }

    // Check file size
    if (file.size > maxSizeInBytes) {
      return false;
    }

    return true;
  },

  // String length validation
  isValidStringLength: function (str, minLength, maxLength) {
    if (!str) return minLength === 0;
    return str.length >= minLength && str.length <= maxLength;
  },

  // Required field validation
  isRequired: function (value) {
    return (
      value !== null && value !== undefined && value.toString().trim() !== ""
    );
  },

  // Show validation error
  showError: function (elementId, message) {
    const element = document.getElementById(elementId);
    if (element) {
      // Remove existing error
      const existingError =
        element.parentNode.querySelector(".validation-error");
      if (existingError) {
        existingError.remove();
      }

      // Add new error
      const errorDiv = document.createElement("div");
      errorDiv.className = "validation-error text-danger small mt-1";
      errorDiv.textContent = message;
      element.parentNode.appendChild(errorDiv);

      // Add error class to input
      element.classList.add("is-invalid");
    }
  },

  // Clear validation error
  clearError: function (elementId) {
    const element = document.getElementById(elementId);
    if (element) {
      const existingError =
        element.parentNode.querySelector(".validation-error");
      if (existingError) {
        existingError.remove();
      }
      element.classList.remove("is-invalid");
    }
  },

  // Validate form
  validateForm: function (formId) {
    const form = document.getElementById(formId);
    if (!form) return false;

    let isValid = true;
    const inputs = form.querySelectorAll("input, select, textarea");

    inputs.forEach((input) => {
      const fieldId = input.id;
      const fieldName = input.name;
      const fieldValue = input.value;
      const fieldType = input.type;

      // Clear previous errors
      this.clearError(fieldId);

      // Required validation
      if (input.hasAttribute("required") && !this.isRequired(fieldValue)) {
        this.showError(fieldId, "Trường này là bắt buộc");
        isValid = false;
        return;
      }

      // Email validation
      if (
        fieldType === "email" &&
        fieldValue &&
        !this.isValidEmail(fieldValue)
      ) {
        this.showError(fieldId, "Email không đúng định dạng");
        isValid = false;
        return;
      }

      // Phone validation
      if (
        fieldName &&
        fieldName.toLowerCase().includes("phone") &&
        fieldValue &&
        !this.isValidVietnamesePhoneNumber(fieldValue)
      ) {
        this.showError(fieldId, "Số điện thoại không hợp lệ");
        isValid = false;
        return;
      }

      // Password validation
      if (
        fieldType === "password" &&
        fieldValue &&
        !this.isValidStrongPassword(fieldValue)
      ) {
        this.showError(
          fieldId,
          "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường và số"
        );
        isValid = false;
        return;
      }

      // Name validation
      if (
        fieldName &&
        (fieldName.toLowerCase().includes("name") ||
          fieldName.toLowerCase().includes("customer")) &&
        fieldValue &&
        !this.isValidVietnameseName(fieldValue)
      ) {
        this.showError(fieldId, "Tên không hợp lệ");
        isValid = false;
        return;
      }

      // Address validation
      if (
        fieldName &&
        fieldName.toLowerCase().includes("address") &&
        fieldValue &&
        !this.isValidVietnameseAddress(fieldValue)
      ) {
        this.showError(fieldId, "Địa chỉ không hợp lệ");
        isValid = false;
        return;
      }

      // Number validation
      if (
        fieldName &&
        (fieldName.toLowerCase().includes("price") ||
          fieldName.toLowerCase().includes("amount")) &&
        fieldValue &&
        !this.isValidPositiveNumber(fieldValue)
      ) {
        this.showError(fieldId, "Giá trị phải là số dương");
        isValid = false;
        return;
      }
    });

    return isValid;
  },

  // Real-time validation
  setupRealTimeValidation: function (formId) {
    const form = document.getElementById(formId);
    if (!form) return;

    const inputs = form.querySelectorAll("input, select, textarea");

    inputs.forEach((input) => {
      input.addEventListener("blur", function () {
        const fieldId = this.id;
        const fieldName = this.name;
        const fieldValue = this.value;
        const fieldType = this.type;

        // Clear previous errors
        ValidationHelper.clearError(fieldId);

        // Required validation
        if (
          this.hasAttribute("required") &&
          !ValidationHelper.isRequired(fieldValue)
        ) {
          ValidationHelper.showError(fieldId, "Trường này là bắt buộc");
          return;
        }

        // Email validation
        if (
          fieldType === "email" &&
          fieldValue &&
          !ValidationHelper.isValidEmail(fieldValue)
        ) {
          ValidationHelper.showError(fieldId, "Email không đúng định dạng");
          return;
        }

        // Phone validation
        if (
          fieldName &&
          fieldName.toLowerCase().includes("phone") &&
          fieldValue &&
          !ValidationHelper.isValidVietnamesePhoneNumber(fieldValue)
        ) {
          ValidationHelper.showError(fieldId, "Số điện thoại không hợp lệ");
          return;
        }

        // Password validation
        if (
          fieldType === "password" &&
          fieldValue &&
          !ValidationHelper.isValidStrongPassword(fieldValue)
        ) {
          ValidationHelper.showError(
            fieldId,
            "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường và số"
          );
          return;
        }

        // Name validation
        if (
          fieldName &&
          (fieldName.toLowerCase().includes("name") ||
            fieldName.toLowerCase().includes("customer")) &&
          fieldValue &&
          !ValidationHelper.isValidVietnameseName(fieldValue)
        ) {
          ValidationHelper.showError(fieldId, "Tên không hợp lệ");
          return;
        }

        // Address validation
        if (
          fieldName &&
          fieldName.toLowerCase().includes("address") &&
          fieldValue &&
          !ValidationHelper.isValidVietnameseAddress(fieldValue)
        ) {
          ValidationHelper.showError(fieldId, "Địa chỉ không hợp lệ");
          return;
        }

        // Number validation
        if (
          fieldName &&
          (fieldName.toLowerCase().includes("price") ||
            fieldName.toLowerCase().includes("amount")) &&
          fieldValue &&
          !ValidationHelper.isValidPositiveNumber(fieldValue)
        ) {
          ValidationHelper.showError(fieldId, "Giá trị phải là số dương");
          return;
        }
      });

      input.addEventListener("input", function () {
        const fieldId = this.id;
        ValidationHelper.clearError(fieldId);
      });
    });
  },
};

// Auto-setup validation for forms with data-validation="true"
document.addEventListener("DOMContentLoaded", function () {
  const forms = document.querySelectorAll('form[data-validation="true"]');
  forms.forEach((form) => {
    ValidationHelper.setupRealTimeValidation(form.id);
  });
});
