using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers;

[Authorize(Roles = "Admin")]
public class VaiTroController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<VaiTroController> _logger;

    public VaiTroController(QuanLyCuaHangTheThaoContext db, ILogger<VaiTroController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search = null)
    {
        var query = _db.TblVaiTros.Include(v => v.TblNhanViens).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.STenVaiTro.Contains(search));

        var items = await query.OrderBy(x => x.IMaVaiTro).ToListAsync();
        ViewBag.Search = search;
        ViewBag.TongVaiTro = items.Count;
        ViewBag.TongNhanVien = items.Sum(x => x.TblNhanViens.Count);
        return View(items);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblVaiTro model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _db.TblVaiTros.AnyAsync(v => v.STenVaiTro == model.STenVaiTro.Trim()))
        {
            ModelState.AddModelError("STenVaiTro", "Tên vai trò đã tồn tại.");
            return View(model);
        }

        model.STenVaiTro = model.STenVaiTro.Trim();
        _db.TblVaiTros.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã thêm vai trò.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.TblVaiTros.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblVaiTro model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _db.TblVaiTros.AnyAsync(v => v.STenVaiTro == model.STenVaiTro.Trim() && v.IMaVaiTro != model.IMaVaiTro))
        {
            ModelState.AddModelError("STenVaiTro", "Tên vai trò đã tồn tại.");
            return View(model);
        }

        var item = await _db.TblVaiTros.FindAsync(model.IMaVaiTro);
        if (item == null) return NotFound();

        item.STenVaiTro = model.STenVaiTro.Trim();
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật vai trò.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var item = await _db.TblVaiTros.Include(v => v.TblNhanViens).FirstOrDefaultAsync(v => v.IMaVaiTro == id);
            if (item == null) return RedirectToAction(nameof(Index));

            if (item.TblNhanViens.Any())
            {
                TempData["Error"] = $"Không thể xóa vì có {item.TblNhanViens.Count} nhân viên đang sử dụng vai trò này.";
                return RedirectToAction(nameof(Index));
            }

            _db.TblVaiTros.Remove(item);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Đã xóa vai trò.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Không thể xóa vai trò.";
        }
        return RedirectToAction(nameof(Index));
    }
}
