using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;
using QuanLyCuaHangTheThao.Services;

namespace QuanLyCuaHangTheThao.Controllers;

[Authorize(Roles = "Admin")]
public class NhanVienController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<NhanVienController> _logger;

    public NhanVienController(QuanLyCuaHangTheThaoContext db, ILogger<NhanVienController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search = null, int page = 1)
    {
        const int pageSize = 10;
        var query = _db.TblNhanViens.Include(n => n.IMaVaiTroNavigation).Include(n => n.TblTaiKhoan).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(n => n.STenNv.Contains(search)
                || (n.SDienThoai != null && n.SDienThoai.Contains(search))
                || n.IMaVaiTroNavigation.STenVaiTro.Contains(search));

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(n => n.IMaNv)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.TongNv = total;
        ViewBag.CoTaiKhoan = await _db.TblTaiKhoans.CountAsync();
        ViewBag.TongVaiTro = await _db.TblVaiTros.CountAsync();
        return View(new PaginatedList<TblNhanVien>(items, total, page, pageSize));
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.VaiTros = await _db.TblVaiTros.OrderBy(x => x.IMaVaiTro).ToListAsync();
        return View(new TblNhanVien());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblNhanVien model, string? sTenDangNhap, string? sMatKhau)
    {
        ViewBag.VaiTros = await _db.TblVaiTros.OrderBy(x => x.IMaVaiTro).ToListAsync();
        ModelState.Remove(nameof(TblNhanVien.IMaVaiTroNavigation));
        if (!ModelState.IsValid) return View(model);

        _db.TblNhanViens.Add(model);
        await _db.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(sTenDangNhap) && !string.IsNullOrWhiteSpace(sMatKhau))
        {
            _db.TblTaiKhoans.Add(new TblTaiKhoan
            {
                STenDangNhap = sTenDangNhap.Trim(),
                SMatKhau = PasswordHasher.Hash(sMatKhau),
                IMaNv = model.IMaNv
            });
        }
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã thêm nhân viên.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.TblNhanViens.Include(n => n.TblTaiKhoan).FirstOrDefaultAsync(n => n.IMaNv == id);
        if (item == null) return NotFound();
        ViewBag.VaiTros = await _db.TblVaiTros.OrderBy(x => x.IMaVaiTro).ToListAsync();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblNhanVien model, string? sTenDangNhap, string? sMatKhau, string? sMatKhauMoi)
    {
        ViewBag.VaiTros = await _db.TblVaiTros.OrderBy(x => x.IMaVaiTro).ToListAsync();
        ModelState.Remove(nameof(TblNhanVien.IMaVaiTroNavigation));
        if (!ModelState.IsValid) return View(model);

        var item = await _db.TblNhanViens.Include(n => n.TblTaiKhoan).FirstOrDefaultAsync(n => n.IMaNv == model.IMaNv);
        if (item == null) return NotFound();

        item.STenNv = model.STenNv;
        item.SGioiTinh = model.SGioiTinh;
        item.SDienThoai = model.SDienThoai;
        item.IMaVaiTro = model.IMaVaiTro;
        await _db.SaveChangesAsync();

        // Account handling
        var taiKhoan = item.TblTaiKhoan;
        if (taiKhoan != null)
        {
            // verify current password if provided and changing
            if (!string.IsNullOrEmpty(sMatKhauMoi))
            {
                if (string.IsNullOrEmpty(sMatKhau) || !PasswordHasher.Verify(sMatKhau, taiKhoan.SMatKhau))
                {
                    ModelState.AddModelError(string.Empty, "Mật khẩu hiện tại không đúng khi đổi mật khẩu.");
                    return View(item);
                }
                taiKhoan.SMatKhau = PasswordHasher.Hash(sMatKhauMoi);
            }
            if (!string.IsNullOrWhiteSpace(sTenDangNhap))
                taiKhoan.STenDangNhap = sTenDangNhap.Trim();
            await _db.SaveChangesAsync();
        }
        else if (!string.IsNullOrWhiteSpace(sTenDangNhap) && !string.IsNullOrWhiteSpace(sMatKhauMoi))
        {
            _db.TblTaiKhoans.Add(new TblTaiKhoan
            {
                STenDangNhap = sTenDangNhap.Trim(),
                SMatKhau = PasswordHasher.Hash(sMatKhauMoi),
                IMaNv = item.IMaNv
            });
            await _db.SaveChangesAsync();
        }

        TempData["Success"] = "Đã cập nhật nhân viên.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var item = await _db.TblNhanViens.Include(n => n.TblTaiKhoan).FirstOrDefaultAsync(n => n.IMaNv == id);
            if (item != null)
            {
                if (item.TblTaiKhoan != null) _db.TblTaiKhoans.Remove(item.TblTaiKhoan);
                _db.TblNhanViens.Remove(item);
                await _db.SaveChangesAsync();
            }
            TempData["Success"] = "Đã xóa nhân viên.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Không thể xóa vì nhân viên có dữ liệu liên quan (hóa đơn/phiếu nhập).";
        }
        return RedirectToAction(nameof(Index));
    }
}
