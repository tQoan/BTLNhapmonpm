using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(QuanLyCuaHangTheThaoContext db, ILogger<DashboardController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;
        var fromDate = today.AddDays(-6);

        ViewBag.TongSanPham = await _db.TblSanPhams.CountAsync();
        ViewBag.TongTonKho = (int?)await _db.TblSanPhams.SumAsync(s => (int?)s.ISoLuongTon) ?? 0;
        ViewBag.SoDonHomNay = await _db.TblHoaDons.CountAsync(h => h.DNgayLap.Date == today);
        ViewBag.DoanhThuHomNay = await _db.TblHoaDons
            .Where(h => h.DNgayLap.Date == today)
            .SumAsync(h => (decimal?)h.FThanhToan) ?? 0;

        // 7 ngày gần nhất - server-side grouping
        var weekly = await _db.TblHoaDons
            .Where(h => h.DNgayLap.Date >= fromDate)
            .GroupBy(h => h.DNgayLap.Date)
            .Select(g => new { Day = g.Key, Total = g.Sum(x => x.FThanhToan) })
            .ToListAsync();

        var labels = new List<string>();
        var values = new List<decimal>();
        for (int i = 6; i >= 0; i--)
        {
            var day = today.AddDays(-i);
            labels.Add(day.ToString("dd/MM"));
            values.Add(weekly.FirstOrDefault(w => w.Day.Date == day)?.Total ?? 0);
        }
        ViewBag.ChartLabels = string.Join(",", labels);
        ViewBag.ChartValues = string.Join(",", values.Select(v => v.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture)));

        // Bán chạy
        var banChay = await _db.TblChiTietHoaDons
            .GroupBy(c => c.IMaSp)
            .Select(g => new { Id = g.Key, Tong = g.Sum(x => x.ISoLuong) })
            .OrderByDescending(x => x.Tong)
            .Take(5)
            .ToListAsync();

        var productIds = banChay.Select(b => b.Id).ToList();
        var products = await _db.TblSanPhams
            .Where(s => productIds.Contains(s.IMaSp))
            .ToDictionaryAsync(s => s.IMaSp);

        var banChayList = new List<(string Ten, int SoLuong)>();
        foreach (var item in banChay)
        {
            banChayList.Add((products.TryGetValue(item.Id, out var sp) ? sp.STenSp : "N/A", item.Tong));
        }
        ViewBag.BanChay = banChayList;

        return View();
    }
}
