using System.ComponentModel.DataAnnotations;

namespace Shared.Auth
{
    public class AuthRegisterRequest
    {
            [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
            [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải là 10 chữ số")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Họ và tên là bắt buộc")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ và tên phải từ 2 đến 100 ký tự")]
            public string Fullname { get; set; }

            [Required(ErrorMessage = "Email là bắt buộc")]
            [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
            [StringLength(50, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 50 ký tự")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Mật khẩu phải chứa ít nhất 1 chữ cái in hoa, 1 chữ cái thường và 1 số")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không khớp")]
            public string ConfirmPassword { get; set; }
    }
}
