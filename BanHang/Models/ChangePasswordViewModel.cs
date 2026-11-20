using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangTheThao.Models;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại.")]
    [Display(Name = "Mật khẩu hiện tại")]
    public string MatKhauHienTai { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới.")]
    [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự.")]
    [Display(Name = "Mật khẩu mới")]
    public string MatKhauMoi { get; set; } = null!;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới.")]
    [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
    [Display(Name = "Xác nhận mật khẩu mới")]
    public string XacNhanMatKhau { get; set; } = null!;
}
