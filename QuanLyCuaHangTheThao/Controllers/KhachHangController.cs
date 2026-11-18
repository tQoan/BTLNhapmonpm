using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers;

[Authorize]
public class KhachHangController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<KhachHangController> _logger;

    public KhachHangController(QuanLyCuaHangTheThaoContext db, ILogger<KhachHangController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search = null, int page = 1)
    {
        const int pageSize = 10;
        var query = _db.TblKhachHangs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.STenKh.Contains(search)
                || (x.SDienThoai != null && x.SDienThoai.Contains(search))
                || (x.SDiaChi != null && x.SDiaChi.Contains(search)));

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.IMaKh)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.TongKhach = total;
        return View(new PaginatedList<TblKhachHang>(items, total, page, pageSize));
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblKhachHang model)
    {
        if (!string.IsNullOrWhiteSpace(model.SDienThoai)
            && await _db.TblKhachHangs.AnyAsync(x => x.SDienThoai == model.SDienThoai.Trim()))
        {
            ModelState.AddModelError(nameof(TblKhachHang.SDienThoai), "Số điện thoại đã tồn tại.");
        }

        if (!ModelState.IsValid) return View(model);
        _db.TblKhachHangs.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã thêm khách hàng.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.TblKhachHangs.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblKhachHang model)
    {
        if (!string.IsNullOrWhiteSpace(model.SDienThoai)
            && await _db.TblKhachHangs.AnyAsync(x => x.SDienThoai == model.SDienThoai.Trim() && x.IMaKh != model.IMaKh))
        {
            ModelState.AddModelError(nameof(TblKhachHang.SDienThoai), "Số điện thoại đã tồn tại.");
        }

        if (!ModelState.IsValid) return View(model);
        _db.Update(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật khách hàng.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var item = await _db.TblKhachHangs.FindAsync(id);
            if (item != null) { _db.TblKhachHangs.Remove(item); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Đã xóa khách hàng.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Không thể xóa vì khách hàng có hóa đơn liên quan.";
        }
        return RedirectToAction(nameof(Index));
    }
}
