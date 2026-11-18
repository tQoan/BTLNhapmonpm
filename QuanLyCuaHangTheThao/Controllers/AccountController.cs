using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models;
using QuanLyCuaHangTheThao.Models.Scaffold;
using QuanLyCuaHangTheThao.Services;

namespace QuanLyCuaHangTheThao.Controllers;

public class AccountController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<AccountController> _logger;

    public AccountController(QuanLyCuaHangTheThaoContext db, ILogger<AccountController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "TrangChu");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var taiKhoan = await _db.TblTaiKhoans
            .Include(t => t.IMaNvNavigation)
                .ThenInclude(nv => nv.IMaVaiTroNavigation)
            .FirstOrDefaultAsync(t => t.STenDangNhap == model.TenDangNhap.Trim());

        if (taiKhoan == null || !PasswordHasher.Verify(model.MatKhau, taiKhoan.SMatKhau))
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(model);
        }

        var nhanVien = taiKhoan.IMaNvNavigation;
        if (nhanVien == null)
        {
            ModelState.AddModelError(string.Empty, "Tài khoản không gắn với nhân viên hợp lệ.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, taiKhoan.STenDangNhap),
            new Claim(ClaimTypes.Name, nhanVien.STenNv ?? taiKhoan.STenDangNhap),
            new Claim("iMaNV", nhanVien.IMaNv.ToString()),
            new Claim("sTenDangNhap", taiKhoan.STenDangNhap),
            new Claim(ClaimTypes.Role, nhanVien.IMaVaiTroNavigation?.STenVaiTro ?? "Người dùng")
        };

        _logger.LogInformation("Nhan vien {TenDangNhap} (MaNV {MaNV}) dang nhap thanh cong", taiKhoan.STenDangNhap, nhanVien.IMaNv);

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "TrangChu");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "TrangChu");
    }

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var username = User.FindFirst("sTenDangNhap")?.Value;
        if (string.IsNullOrEmpty(username)) return RedirectToAction("Index", "TrangChu");

        var taiKhoan = await _db.TblTaiKhoans.FindAsync(username);
        if (taiKhoan == null) return RedirectToAction("Index", "TrangChu");

        if (!PasswordHasher.Verify(model.MatKhauHienTai, taiKhoan.SMatKhau))
        {
            ModelState.AddModelError("MatKhauHienTai", "Mật khẩu hiện tại không đúng.");
            return View(model);
        }

        taiKhoan.SMatKhau = PasswordHasher.Hash(model.MatKhauMoi);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Đổi mật khẩu thành công.";
        return RedirectToAction(nameof(ChangePassword));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
