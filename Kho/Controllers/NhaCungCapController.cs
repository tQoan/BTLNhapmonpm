using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers;

[Authorize]
public class NhaCungCapController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<NhaCungCapController> _logger;

    public NhaCungCapController(QuanLyCuaHangTheThaoContext db, ILogger<NhaCungCapController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search = null, int page = 1)
    {
        const int pageSize = 10;
        var query = _db.TblNhaCungCaps.Include(n => n.TblSanPhams).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.STenNcc.Contains(search)
                || (x.SDienThoai != null && x.SDienThoai.Contains(search))
                || (x.SDiaChi != null && x.SDiaChi.Contains(search)));

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.IMaNcc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.TongNcc = total;
        ViewBag.TongSanPham = await _db.TblSanPhams.CountAsync();
        return View(new PaginatedList<TblNhaCungCap>(items, total, page, pageSize));
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblNhaCungCap model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.TblNhaCungCaps.Add(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã thêm nhà cung cấp.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.TblNhaCungCaps.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblNhaCungCap model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.Update(model);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Đã cập nhật nhà cung cấp.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var item = await _db.TblNhaCungCaps.FindAsync(id);
            if (item != null) { _db.TblNhaCungCaps.Remove(item); await _db.SaveChangesAsync(); }
            TempData["Success"] = "Đã xóa nhà cung cấp.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Không thể xóa vì nhà cung cấp đang được sử dụng.";
        }
        return RedirectToAction(nameof(Index));
    }
}
