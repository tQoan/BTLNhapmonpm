using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers;

[Authorize]
public class KichThuocController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<KichThuocController> _logger;

    public KichThuocController(QuanLyCuaHangTheThaoContext db, ILogger<KichThuocController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search = null)
    {
        var query = _db.TblKichThuocs.Include(s => s.TblSanPhamTblKichThuocs).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.STenSize.Contains(search));

        var items = await query.OrderBy(x => x.IMaSize).ToListAsync();
        ViewBag.Search = search;
        ViewBag.TongSize = items.Count;
        ViewBag.TongSanPham = items.Sum(x => x.TblSanPhamTblKichThuocs.Count);
        return View(items);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblKichThuoc model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _db.TblKichThuocs.AnyAsync(s => s.STenSize == model.STenSize.Trim()))
        {
            ModelState.AddModelError("STenSize", "Tên size đã tồn tại.");
            return View(model);
        }

        model.STenSize = model.STenSize.Trim();
        _db.TblKichThuocs.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã thêm kích thước.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.TblKichThuocs.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblKichThuoc model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _db.TblKichThuocs.AnyAsync(s => s.STenSize == model.STenSize.Trim() && s.IMaSize != model.IMaSize))
        {
            ModelState.AddModelError("STenSize", "Tên size đã tồn tại.");
            return View(model);
        }

        var item = await _db.TblKichThuocs.FindAsync(model.IMaSize);
        if (item == null) return NotFound();

        item.STenSize = model.STenSize.Trim();
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật kích thước.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var item = await _db.TblKichThuocs.FindAsync(id);
            if (item != null) { _db.TblKichThuocs.Remove(item); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Đã xóa kích thước.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Không thể xóa vì size đang được sử dụng.";
        }
        return RedirectToAction(nameof(Index));
    }
}
