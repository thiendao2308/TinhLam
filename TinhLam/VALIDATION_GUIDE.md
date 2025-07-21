# Hướng dẫn sử dụng hệ thống Validation cho dự án TinhLam

## Tổng quan

Dự án TinhLam đã được tích hợp một hệ thống validation toàn diện bao gồm:

### 1. Server-side Validation (C#)

- **Custom Validation Attributes**: Các attribute tùy chỉnh cho validation tiếng Việt
- **ViewModels với Validation**: Tất cả ViewModel đều có validation đầy đủ
- **BaseController**: Controller cơ sở với các phương thức validation helper

### 2. Client-side Validation (JavaScript)

- **ValidationHelper**: Thư viện JavaScript cho validation real-time
- **CSS Styling**: Styling cho validation errors và success states

## Các Custom Validation Attributes

### VietnamesePhoneNumberAttribute

```csharp
[VietnamesePhoneNumber(ErrorMessage = "Số điện thoại không hợp lệ")]
public string PhoneNumber { get; set; }
```

### VietnameseNameAttribute

```csharp
[VietnameseName(ErrorMessage = "Tên không hợp lệ")]
public string CustomerName { get; set; }
```

### StrongPasswordAttribute

```csharp
[StrongPassword(ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường và số")]
public string Password { get; set; }
```

### VietnameseAddressAttribute

```csharp
[VietnameseAddress(ErrorMessage = "Địa chỉ không hợp lệ")]
public string Address { get; set; }
```

### PositiveNumberAttribute

```csharp
[PositiveNumber(ErrorMessage = "Giá trị phải lớn hơn 0")]
public decimal Price { get; set; }
```

### ImageFileAttribute

```csharp
[ImageFile(ErrorMessage = "File ảnh không hợp lệ")]
public IFormFile ImageFile { get; set; }
```

## Cách sử dụng trong Controller

### 1. Kế thừa BaseController

```csharp
public class ProductsController : BaseController
{
    [HttpPost]
    public IActionResult Create(ProductVM model)
    {
        if (!ValidateModel(model))
        {
            return View(model);
        }

        // Xử lý logic tạo sản phẩm
        return RedirectToAction("Index");
    }
}
```

### 2. Sử dụng ValidationService

```csharp
public class UsersController : BaseController
{
    [HttpPost]
    public IActionResult Register(RegisterVM model)
    {
        if (!ValidationService.IsValid(model))
        {
            var errors = ValidationService.GetValidationErrors(model);
            foreach (var error in errors)
            {
                AddModelError(error);
            }
            return View(model);
        }

        // Xử lý đăng ký
        return RedirectToAction("Login");
    }
}
```

## Cách sử dụng trong View

### 1. Thêm CSS và JavaScript

```html
<link rel="stylesheet" href="~/css/validation.css" />
<script src="~/js/validation.js"></script>
```

### 2. Form với validation

```html
<form
  id="registerForm"
  data-validation="true"
  asp-action="Register"
  method="post"
>
  <div class="form-group">
    <label asp-for="CustomerName">Tên khách hàng</label>
    <input asp-for="CustomerName" class="form-control name-input" />
    <span asp-validation-for="CustomerName" class="text-danger"></span>
  </div>

  <div class="form-group">
    <label asp-for="Email">Email</label>
    <input asp-for="Email" type="email" class="form-control email-input" />
    <span asp-validation-for="Email" class="text-danger"></span>
  </div>

  <div class="form-group">
    <label asp-for="PhoneNumber">Số điện thoại</label>
    <input asp-for="PhoneNumber" class="form-control phone-input" />
    <span asp-validation-for="PhoneNumber" class="text-danger"></span>
  </div>

  <button type="submit" class="btn btn-primary">Đăng ký</button>
</form>
```

### 3. JavaScript validation

```javascript
// Setup real-time validation
ValidationHelper.setupRealTimeValidation("registerForm");

// Validate form before submit
document
  .getElementById("registerForm")
  .addEventListener("submit", function (e) {
    if (!ValidationHelper.validateForm("registerForm")) {
      e.preventDefault();
      return false;
    }
  });
```

## Các ViewModel đã được validation

### 1. RegisterVM

- Tên khách hàng (VietnameseName)
- Tên đăng nhập (StringLength, RegularExpression)
- Mật khẩu (StrongPassword)
- Xác nhận mật khẩu (Compare)
- Email (EmailAddress)
- Số điện thoại (VietnamesePhoneNumber)
- Địa chỉ (VietnameseAddress)

### 2. LoginVM

- Tên đăng nhập (Required, StringLength)
- Mật khẩu (Required, DataType)

### 3. DatHangVM

- Tên khách hàng (VietnameseName)
- Số điện thoại (VietnamesePhoneNumber)
- Địa chỉ (VietnameseAddress)
- Tổng tiền (PositiveNumber)
- Các trường địa chỉ (StringLength)

### 4. ProductVM

- Tên sản phẩm (Required, StringLength)
- Giá (PositiveNumber, Range)
- Mô tả (StringLength)
- Hình ảnh (ImageFile)
- Danh mục (Required, Range)

### 5. CategoryVM

- Tên danh mục (Required, StringLength, RegularExpression)
- Mô tả (StringLength)
- Hình ảnh (ImageFile)

### 6. PostVM

- Tiêu đề (Required, StringLength)
- Nội dung (Required, StringLength)
- Tóm tắt (StringLength)
- Hình ảnh (ImageFile)
- Tác giả (Required, StringLength)

### 7. OrderVM

- Tên khách hàng (VietnameseName)
- Số điện thoại (VietnamesePhoneNumber)
- Địa chỉ (VietnameseAddress)
- Tổng tiền (PositiveNumber, Range)
- Các trường khác (StringLength, Range)

### 8. OrderDetailVM

- ID đơn hàng (Required, Range)
- ID sản phẩm (Required, Range)
- Số lượng (Required, Range)
- Đơn giá (PositiveNumber, Range)
- Thành tiền (PositiveNumber, Range)

### 9. ProductSizeVM

- ID sản phẩm (Required, Range)
- Kích thước (Required, StringLength)
- Số lượng tồn kho (Required, Range)

### 10. ChatMessageVM

- ID người gửi (Required, Range)
- ID người nhận (Required, Range)
- Nội dung tin nhắn (Required, StringLength)

### 11. HomeBannerVM

- Tiêu đề (Required, StringLength)
- Mô tả (StringLength)
- Hình ảnh (Required, ImageFile)
- Liên kết (StringLength, Url)
- Thứ tự hiển thị (Required, Range)

### 12. RewardPointVM

- ID khách hàng (Required, Range)
- Số điểm (Required, Range)
- Loại giao dịch (Required, StringLength)
- Mô tả (StringLength)

## Validation Rules

### Email

- Định dạng email hợp lệ
- Độ dài tối đa: 100 ký tự

### Số điện thoại Việt Nam

- Định dạng: 0xxxxxxxxx hoặc +84xxxxxxxxx
- Các đầu số: 03x, 05x, 07x, 08x, 09x

### Mật khẩu mạnh

- Ít nhất 8 ký tự
- Ít nhất 1 chữ hoa
- Ít nhất 1 chữ thường
- Ít nhất 1 số

### Tên tiếng Việt

- Chỉ chứa chữ cái và dấu cách
- Độ dài tối thiểu: 2 ký tự
- Độ dài tối đa: 100 ký tự

### Địa chỉ

- Độ dài tối thiểu: 10 ký tự
- Độ dài tối đa: 200 ký tự

### File ảnh

- Định dạng: jpg, jpeg, png, gif, webp
- Kích thước tối đa: 5MB

### Số dương

- Phải lớn hơn 0
- Hỗ trợ decimal

## Lưu ý quan trọng

1. **Server-side validation là bắt buộc** để đảm bảo an toàn
2. **Client-side validation** chỉ để cải thiện trải nghiệm người dùng
3. **Luôn kiểm tra ModelState.IsValid** trong controller
4. **Sử dụng BaseController** để có các helper methods
5. **Thêm data-validation="true"** vào form để kích hoạt real-time validation

## Troubleshooting

### Validation không hoạt động

1. Kiểm tra đã include CSS và JS files
2. Kiểm tra form có data-validation="true"
3. Kiểm tra ModelState.IsValid trong controller

### Custom validation không hoạt động

1. Kiểm tra đã kế thừa ValidationAttribute
2. Kiểm tra logic trong IsValid method
3. Kiểm tra error message

### Client-side validation không hoạt động

1. Kiểm tra console errors
2. Kiểm tra ValidationHelper đã được load
3. Kiểm tra form ID và field IDs
