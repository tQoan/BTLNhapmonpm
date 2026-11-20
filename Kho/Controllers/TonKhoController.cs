using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers;

[Authorize]
public class TonKhoController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<TonKhoController> _logger;

    public TonKhoController(QuanLyCuaHangTheThaoContext db, ILogger<TonKhoController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search = null, int page = 1)
    {
        const int pageSize = 10;
        var query = _db.VTonKhos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.STenSp.Contains(search) || t.STenSize.Contains(search) || t.STenMau.Contains(search));

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(t => t.IMaSp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.TongMatHang = total;

        var statsQuery = _db.VTonKhos.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            statsQuery = statsQuery.Where(t => t.STenSp.Contains(search) || t.STenSize.Contains(search) || t.STenMau.Contains(search));
        var all = await statsQuery.ToListAsync();

        ViewBag.TongTon = all.Sum(t => t.ISoLuongTon ?? 0);
        ViewBag.TongNhap = all.Sum(t => t.FTongNhap);
        ViewBag.SapHet = all.Count(t => (t.ISoLuongTon ?? 0) <= 5);
        ViewBag.HetHang = all.Count(t => (t.ISoLuongTon ?? 0) == 0);
        return View(new PaginatedList<VTonKho>(items, total, page, pageSize));
    }

    public async Task<IActionResult> HanDung()
    {
        return View(await _db.VThis.ToListAsync());
    }
}
