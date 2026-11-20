using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers;

[Authorize(Roles = "Admin,Bán hàng")]
public class BanHangController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<BanHangController> _logger;

    public BanHangController(QuanLyCuaHangTheThaoContext db, ILogger<BanHangController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        await PopulateData();
        return View(new HoaDonViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(HoaDonViewModel model)
    {
        await PopulateData();

        var validLines = model.Lines.Where(l => l.IMaSp > 0 && l.IMaSize > 0 && l.IMaMau > 0 && l.ISoLuong > 0).ToList();
        if (!validLines.Any())
            ModelState.AddModelError(string.Empty, "Vui lòng thêm ít nhất một sản phẩm vào hóa đơn.");
        if (model.FGiamGia < 0 || model.FGiamGia > 100)
            ModelState.AddModelError(nameof(HoaDonViewModel.FGiamGia), "Giảm giá phải từ 0 đến 100%.");

        if (!ModelState.IsValid) return View(model);

        // Verify stock before saving
        foreach (var line in validLines)
        {
            var tk = await _db.TblTonKhoChiTiets.FirstOrDefaultAsync(
                t => t.IMaSp == line.IMaSp && t.IMaSize == line.IMaSize && t.IMaMau == line.IMaMau);
            var ton = tk?.ISoLuongTon ?? 0;
            if (line.ISoLuong > ton)
            {
                var sp = await _db.TblSanPhams.FindAsync(line.IMaSp);
                ModelState.AddModelError(string.Empty, $"Sản phẩm '{sp?.STenSp}' tồn kho chỉ còn {ton}.");
                return View(model);
            }
        }

        int maNV = int.Parse(User.FindFirstValue("iMaNV") ?? "0");

        decimal tongGoc = validLines.Sum(l => l.ISoLuong * l.FDonGia);
        decimal thanhToan = tongGoc * (1 - model.FGiamGia / 100m);
        decimal tienTraLai = 0;
        if (model.SPhuongThucTt == "Tiền mặt" && model.FTienKhachTra > thanhToan)
            tienTraLai = model.FTienKhachTra - thanhToan;

        var hd = new TblHoaDon
        {
            DNgayLap = DateTime.Now,
            IMaKh = model.IMaKh,
            IMaNv = maNV,
            FGiamGia = model.FGiamGia,
            FTongTien = tongGoc,
            FThanhToan = thanhToan,
            SPhuongThucTt = model.SPhuongThucTt,
            FTienKhachTra = model.FTienKhachTra,
            FTienTraLai = tienTraLai
        };
        _db.TblHoaDons.Add(hd);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Nhan vien {MaNV} tao hoa don #{MaHD}", maNV, hd.IMaHd);

        foreach (var line in validLines)
        {
            _db.TblChiTietHoaDons.Add(new TblChiTietHoaDon
            {
                IMaHd = hd.IMaHd,
                IMaSp = line.IMaSp,
                IMaSize = line.IMaSize,
                IMaMau = line.IMaMau,
                ISoLuong = line.ISoLuong,
                FDonGia = line.FDonGia
            });
        }

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Trigger may throw (oversell). Undo header.
            _db.TblHoaDons.Remove(hd);
            await _db.SaveChangesAsync();
            _logger.LogWarning("Luu hoa don #{MaHD} that bai do ton kho khong du", hd.IMaHd);

            var msg = "Không thể lưu hóa đơn do tồn kho không đủ.";
            ModelState.AddModelError(string.Empty, msg);
            return View(model);
        }

        TempData["Success"] = $"Đã lưu hóa đơn #{hd.IMaHd}.";
        return RedirectToAction("ChiTiet", "HoaDon", new { id = hd.IMaHd });
    }

    private async Task PopulateData()
    {
        ViewBag.KhachHangs = await _db.TblKhachHangs.OrderBy(x => x.STenKh).ToListAsync();
        ViewBag.SanPhams = await _db.TblSanPhams.OrderBy(x => x.STenSp).ToListAsync();

        // For variant prices + stock
        var sizeMap = await _db.TblSanPhamTblKichThuocs.ToListAsync();
        var mauMap = await _db.TblSanPhamTblMauSacs.ToListAsync();
        var sizes = await _db.TblKichThuocs.ToDictionaryAsync(x => x.IMaSize);
        var maus = await _db.TblMauSacs.ToDictionaryAsync(x => x.IMaMau);
        var tonKho = await _db.TblTonKhoChiTiets.ToListAsync();

        var variants = new Dictionary<int, object>();
        foreach (var sp in await _db.TblSanPhams.ToListAsync())
        {
            var spSizes = sizeMap.Where(x => x.IMaSp == sp.IMaSp)
                .Select(x => new { id = x.IMaSize, name = sizes[x.IMaSize].STenSize }).ToList();
            var spMaus = mauMap.Where(x => x.IMaSp == sp.IMaSp)
                .Select(x => new { id = x.IMaMau, name = maus[x.IMaMau].STenMau }).ToList();
            var tkMap = tonKho.Where(t => t.IMaSp == sp.IMaSp)
                .ToDictionary(t => $"{t.IMaSize}-{t.IMaMau}", t => t.ISoLuongTon);
            variants[sp.IMaSp] = new
            {
                gb = sp.FGiaBan,
                sizes = spSizes,
                maus = spMaus,
                ton = tkMap
            };
        }
        ViewBag.VariantsJson = System.Text.Json.JsonSerializer.Serialize(variants);
    }
}
