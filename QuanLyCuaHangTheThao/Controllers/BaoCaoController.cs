using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers;

[Authorize(Roles = "Admin,Kế toán")]
public class BaoCaoController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<BaoCaoController> _logger;

    public BaoCaoController(QuanLyCuaHangTheThaoContext db, ILogger<BaoCaoController> logger)
    {
        _db = db;
        _logger = logger;
    }

    private IQueryable<VBaoCaoDoanhThu> ApplyFilter(IQueryable<VBaoCaoDoanhThu> query, string? tuNgay, string? denNgay)
    {
        if (DateTime.TryParse(tuNgay, out var tu))
            query = query.Where(v => v.DNgay >= DateOnly.FromDateTime(tu));
        if (DateTime.TryParse(denNgay, out var den))
            query = query.Where(v => v.DNgay <= DateOnly.FromDateTime(den));
        return query;
    }

    public async Task<IActionResult> Index(string? tuNgay, string? denNgay)
    {
        var query = ApplyFilter(_db.VBaoCaoDoanhThus.AsQueryable(), tuNgay, denNgay);

        ViewBag.TuNgay = tuNgay;
        ViewBag.DenNgay = denNgay;
        ViewBag.TongDoanhThu = await query.SumAsync(v => (decimal?)v.FDoanhThuThucThu) ?? 0;
        ViewBag.TongGiaVon = await query.SumAsync(v => (decimal?)v.FGiaVon) ?? 0;
        ViewBag.TongLoiNhuan = await query.SumAsync(v => (decimal?)v.FLoiNhuanGop) ?? 0;
        ViewBag.SoHoaDon = await query.Select(v => v.IMaHd).Distinct().CountAsync();

        // Group by day in the database
        var byDay = await query
            .Where(v => v.DNgay != null)
            .GroupBy(v => v.DNgay!.Value)
            .Select(g => new
            {
                Day = g.Key,
                DoanhThu = g.Sum(x => (decimal?)x.FDoanhThuThucThu) ?? 0,
                GiaVon = g.Sum(x => (decimal?)x.FGiaVon) ?? 0,
                LoiNhuan = g.Sum(x => (decimal?)x.FLoiNhuanGop) ?? 0
            })
            .OrderBy(x => x.Day)
            .ToListAsync();

        ViewBag.ChartLabels = string.Join(",", byDay.Select(x => x.Day.ToString("dd/MM", CultureInfo.InvariantCulture)));
        ViewBag.ChartValues = string.Join(",", byDay.Select(x => x.DoanhThu.ToString("0.##", CultureInfo.InvariantCulture)));

        return View(byDay);
    }

    [HttpGet]
    public async Task<IActionResult> ExportCsv(string? tuNgay, string? denNgay)
    {
        var query = ApplyFilter(_db.VBaoCaoDoanhThus.AsQueryable(), tuNgay, denNgay);

        var rows = await query
            .Where(v => v.DNgay != null)
            .GroupBy(v => v.DNgay!.Value)
            .Select(g => new
            {
                Day = g.Key,
                DoanhThu = g.Sum(x => (decimal?)x.FDoanhThuThucThu) ?? 0,
                GiaVon = g.Sum(x => (decimal?)x.FGiaVon) ?? 0,
                LoiNhuan = g.Sum(x => (decimal?)x.FLoiNhuanGop) ?? 0,
                SoHoaDon = g.Select(x => x.IMaHd).Distinct().Count()
            })
            .OrderBy(x => x.Day)
            .ToListAsync();

        var sb = new StringBuilder();
        sb.AppendLine("\"Ngay\",\"So hoa don\",\"Gia von\",\"Doanh thu thuc thu\",\"Loi nhuan gop\"");

        foreach (var r in rows)
        {
            sb.AppendLine($"\"{r.Day:dd/MM/yyyy}\",{r.SoHoaDon},{r.GiaVon.ToString("0.##", CultureInfo.InvariantCulture)},{r.DoanhThu.ToString("0.##", CultureInfo.InvariantCulture)},{r.LoiNhuan.ToString("0.##", CultureInfo.InvariantCulture)}");
        }

        _logger.LogInformation("Xuat CSV bao cao doanh thu {TuNgay} -> {DenNgay}: {SoDong} dong", tuNgay, denNgay, rows.Count);

        // UTF-8 with BOM de Excel mo dung tieng Viet
        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        var fileName = $"bao-cao-doanh-thu_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        return File(bytes, "text/csv; charset=utf-8", fileName);
    }
}
