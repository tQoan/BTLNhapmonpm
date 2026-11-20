using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers;

[Authorize]
public class MauSacController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<MauSacController> _logger;

    public MauSacController(QuanLyCuaHangTheThaoContext db, ILogger<MauSacController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search = null)
    {
        var query = _db.TblMauSacs.Include(m => m.TblSanPhamTblMauSacs).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.STenMau.Contains(search));

        var items = await query.OrderBy(x => x.IMaMau).ToListAsync();
        ViewBag.Search = search;
        ViewBag.TongMau = items.Count;
        ViewBag.TongSanPham = items.Sum(x => x.TblSanPhamTblMauSacs.Count);
        return View(items);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblMauSac model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _db.TblMauSacs.AnyAsync(m => m.STenMau == model.STenMau.Trim()))
        {
            ModelState.AddModelError("STenMau", "Tên màu đã tồn tại.");
            return View(model);
        }

        model.STenMau = model.STenMau.Trim();
        _db.TblMauSacs.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã thêm màu sắc.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.TblMauSacs.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblMauSac model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _db.TblMauSacs.AnyAsync(m => m.STenMau == model.STenMau.Trim() && m.IMaMau != model.IMaMau))
        {
            ModelState.AddModelError("STenMau", "Tên màu đã tồn tại.");
            return View(model);
        }

        var item = await _db.TblMauSacs.FindAsync(model.IMaMau);
        if (item == null) return NotFound();

        item.STenMau = model.STenMau.Trim();
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật màu sắc.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var item = await _db.TblMauSacs.FindAsync(id);
            if (item != null) { _db.TblMauSacs.Remove(item); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Đã xóa màu sắc.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Không thể xóa vì màu sắc đang được sử dụng.";
        }
        return RedirectToAction(nameof(Index));
    }
}
