using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers;

[Authorize]
public class HoaDonController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<HoaDonController> _logger;

    public HoaDonController(QuanLyCuaHangTheThaoContext db, ILogger<HoaDonController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? tuNgay, string? denNgay, string? search, int page = 1)
    {
        const int pageSize = 10;
        var query = _db.TblHoaDons
            .Include(h => h.IMaKhNavigation)
            .Include(h => h.IMaNvNavigation).AsQueryable();

        if (DateTime.TryParse(tuNgay, out var tu))
            query = query.Where(h => h.DNgayLap.Date >= tu.Date);
        if (DateTime.TryParse(denNgay, out var den))
            query = query.Where(h => h.DNgayLap.Date <= den.Date);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(h => h.IMaHd.ToString().Contains(search)
                || (h.IMaKhNavigation != null && h.IMaKhNavigation.STenKh.Contains(search))
                || h.SPhuongThucTt.Contains(search));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(h => h.IMaHd)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.TuNgay = tuNgay;
        ViewBag.DenNgay = denNgay;
        ViewBag.Search = search;
        ViewBag.TongHoaDon = total;
        ViewBag.TongDoanhThu = await query.SumAsync(h => h.FThanhToan);
        ViewBag.TongGiamGia = await query.SumAsync(h => h.FTongTien - h.FThanhToan);
        return View(new PaginatedList<TblHoaDon>(items, total, page, pageSize));
    }

    public async Task<IActionResult> ChiTiet(int id)
    {
        var hd = await _db.TblHoaDons
            .Include(h => h.IMaKhNavigation)
            .Include(h => h.IMaNvNavigation)
            .FirstOrDefaultAsync(h => h.IMaHd == id);
        if (hd == null) return NotFound();

        var lines = await _db.TblChiTietHoaDons
            .Where(c => c.IMaHd == id)
            .Include(c => c.TblSanPhamTblKichThuoc).ThenInclude(j => j.IMaSizeNavigation)
            .Include(c => c.TblSanPhamTblKichThuoc).ThenInclude(j => j.IMaSpNavigation)
            .Include(c => c.TblSanPhamTblMauSac).ThenInclude(j => j.IMaMauNavigation)
            .ToListAsync();

        ViewBag.HoaDon = hd;
        return View(lines);
    }

    public async Task<IActionResult> In(int id)
    {
        var data = await _db.VInHoaDons.Where(v => v.IMaHd == id).ToListAsync();
        if (!data.Any()) return NotFound();
        ViewBag.MaHD = id;
        ViewBag.NgayLap = data.First().DNgayLap;
        ViewBag.KhachHang = data.First().STenKh;
        ViewBag.DienThoaiKH = data.First().SDienThoaiKh;
        ViewBag.NhanVien = data.First().STenNv;
        ViewBag.GiamGia = data.First().FGiamGia;
        ViewBag.TongTien = data.First().FTongTien;
        ViewBag.ThanhToan = data.First().FThanhToan;
        ViewBag.PhuongThuc = data.First().SPhuongThucTt;
        ViewBag.TienKhachTra = data.First().FTienKhachTra;
        ViewBag.TienTraLai = data.First().FTienTraLai;
        return View(data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var hd = await _db.TblHoaDons.FindAsync(id);
            if (hd == null) return NotFound();

            var lines = await _db.TblChiTietHoaDons
                .Where(c => c.IMaHd == id)
                .ToListAsync();

            // Hoan tra ton kho cho tung dong chi tiet (undo trigger cua luc ban)
            foreach (var line in lines)
            {
                var tk = await _db.TblTonKhoChiTiets
                    .FirstOrDefaultAsync(t => t.IMaSp == line.IMaSp && t.IMaSize == line.IMaSize && t.IMaMau == line.IMaMau);
                if (tk != null)
                    tk.ISoLuongTon += line.ISoLuong;
            }

            _db.TblChiTietHoaDons.RemoveRange(lines);
            await _db.SaveChangesAsync();

            _db.TblHoaDons.Remove(hd);
            await _db.SaveChangesAsync();

            _logger.LogWarning("Hoa don #{MaHD} da bi xoa (hoan tra ton kho)", id);
            TempData["Success"] = $"Đã xóa hóa đơn #{id} và hoàn trả tồn kho.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            _logger.LogError("Khong the xoa hoa don #{MaHD} do rang buoc du lieu", id);
            TempData["Error"] = "Không thể xóa hóa đơn do ràng buộc dữ liệu liên quan.";
            return RedirectToAction(nameof(Index));
        }
    }
}
