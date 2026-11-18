using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;
using QuanLyCuaHangTheThao.Services;

namespace QuanLyCuaHangTheThao.Controllers;

[Authorize(Roles = "Admin")]
public class CaLamViecController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<CaLamViecController> _logger;

    public CaLamViecController(QuanLyCuaHangTheThaoContext db, ILogger<CaLamViecController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search = null)
    {
        var query = _db.TblCaLamViecs.Include(c => c.TblLichLamViecs).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.STenCa.Contains(search));

        var items = await query.OrderBy(c => c.DGioBatDau).ThenBy(c => c.IMaCa).ToListAsync();
        ViewBag.Search = search;
        ViewBag.TongCa = items.Count;
        ViewBag.TongLuoc = items.Sum(c => c.TblLichLamViecs.Count);
        return View(items);
    }

    public IActionResult Create() => View(new TblCaLamViec
    {
        DGioBatDau = new TimeSpan(7, 0, 0),
        DGioKetThuc = new TimeSpan(15, 0, 0)
    });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TblCaLamViec model)
    {
        if (!KiemTraGioHopLe(model)) return View(model);
        if (!ModelState.IsValid) return View(model);

        if (await _db.TblCaLamViecs.AnyAsync(c => c.STenCa == model.STenCa.Trim()))
        {
            ModelState.AddModelError("STenCa", "Tên ca đã tồn tại.");
            return View(model);
        }

        model.STenCa = model.STenCa.Trim();
        _db.TblCaLamViecs.Add(model);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Them ca lam viec {TenCa}", model.STenCa);
        TempData["Success"] = "Đã thêm ca làm việc.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.TblCaLamViecs.FindAsync(id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TblCaLamViec model)
    {
        if (!KiemTraGioHopLe(model)) return View(model);
        if (!ModelState.IsValid) return View(model);

        if (await _db.TblCaLamViecs.AnyAsync(c => c.STenCa == model.STenCa.Trim() && c.IMaCa != model.IMaCa))
        {
            ModelState.AddModelError("STenCa", "Tên ca đã tồn tại.");
            return View(model);
        }

        var item = await _db.TblCaLamViecs.FindAsync(model.IMaCa);
        if (item == null) return NotFound();

        item.STenCa = model.STenCa.Trim();
        item.DGioBatDau = model.DGioBatDau;
        item.DGioKetThuc = model.DGioKetThuc;
        await _db.SaveChangesAsync();
        _logger.LogInformation("Cap nhat ca lam viec {TenCa}", item.STenCa);
        TempData["Success"] = "Đã cập nhật ca làm việc.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var item = await _db.TblCaLamViecs.Include(c => c.TblLichLamViecs).FirstOrDefaultAsync(c => c.IMaCa == id);
            if (item == null) return RedirectToAction(nameof(Index));

            if (item.TblLichLamViecs.Any())
            {
                TempData["Error"] = $"Không thể xóa vì có {item.TblLichLamViecs.Count} lịch đang dùng ca \"{item.STenCa}\".";
                return RedirectToAction(nameof(Index));
            }

            _db.TblCaLamViecs.Remove(item);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Xoa ca lam viec {TenCa}", item.STenCa);
            TempData["Success"] = "Đã xóa ca làm việc.";
        }
        catch (DbUpdateException)
        {
            _logger.LogError("Khong the xoa ca lam viec {Id}", id);
            TempData["Error"] = "Không thể xóa ca làm việc.";
        }
        return RedirectToAction(nameof(Index));
    }

    private bool KiemTraGioHopLe(TblCaLamViec model)
    {
        if (LichLamViecTinh.LaCaHopLe(model.DGioBatDau, model.DGioKetThuc)) return true;

        ModelState.AddModelError(nameof(TblCaLamViec.DGioKetThuc), "Giờ kết thúc phải khác giờ bắt đầu.");
        return false;
    }
}