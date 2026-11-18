using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers.Api;

[ApiController]
[Route("api/banhang")]
[Authorize(Roles = "Admin,Bán hàng")]
public class BanHangApiController : ControllerBase
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<BanHangApiController> _logger;

    public BanHangApiController(QuanLyCuaHangTheThaoContext db, ILogger<BanHangApiController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // POST /api/banhang - tao hoa don moi
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateHoaDonRequest req)
    {
        if (req == null || req.Lines == null || !req.Lines.Any())
            return BadRequest(new { message = "Vui long them it nhat mot san pham." });
        if (req.FGiamGia < 0 || req.FGiamGia > 100)
            return BadRequest(new { message = "Giam gia phai tu 0 den 100%." });

        var validLines = req.Lines.Where(l => l.IMaSp > 0 && l.IMaSize > 0 && l.IMaMau > 0 && l.ISoLuong > 0).ToList();
        if (!validLines.Any())
            return BadRequest(new { message = "Vui long them it nhat mot san pham voi size/mau/so luong hop le." });

        // Kiem tra ton kho truoc
        foreach (var line in validLines)
        {
            var tk = await _db.TblTonKhoChiTiets.FirstOrDefaultAsync(
                t => t.IMaSp == line.IMaSp && t.IMaSize == line.IMaSize && t.IMaMau == line.IMaMau);
            var ton = tk?.ISoLuongTon ?? 0;
            if (line.ISoLuong > ton)
            {
                var sp = await _db.TblSanPhams.FindAsync(line.IMaSp);
                return BadRequest(new { message = $"San pham '{sp?.STenSp}' ton kho chi con {ton}." });
            }
        }

        int maNV = int.Parse(User.FindFirstValue("iMaNV") ?? "0");
        if (maNV == 0)
            return Unauthorized(new { message = "Khong xac dinh duoc nhan vien dang nhap." });

        decimal tongGoc = validLines.Sum(l => l.ISoLuong * l.FDonGia);
        decimal thanhToan = tongGoc * (1 - req.FGiamGia / 100m);
        decimal tienTraLai = 0;
        if (req.SPhuongThucTt == "Tien mat" && req.FTienKhachTra > thanhToan)
            tienTraLai = req.FTienKhachTra - thanhToan;

        var hd = new TblHoaDon
        {
            DNgayLap = DateTime.Now,
            IMaKh = req.IMaKh,
            IMaNv = maNV,
            FGiamGia = req.FGiamGia,
            FTongTien = tongGoc,
            FThanhToan = thanhToan,
            SPhuongThucTt = req.SPhuongThucTt,
            FTienKhachTra = req.FTienKhachTra,
            FTienTraLai = tienTraLai
        };
        _db.TblHoaDons.Add(hd);
        await _db.SaveChangesAsync();

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
            _db.TblHoaDons.Remove(hd);
            await _db.SaveChangesAsync();
            _logger.LogWarning("API tao hoa don that bai do ton kho khong du");
            return BadRequest(new { message = "Khong the luu hoa don do ton kho khong du." });
        }

        _logger.LogInformation("API: nhan vien {MaNV} tao hoa don #{MaHD}", maNV, hd.IMaHd);
        return Created($"/api/banhang/{hd.IMaHd}", new { hd.IMaHd, hd.FTongTien, hd.FThanhToan, hd.FTienTraLai });
    }

    public class CreateHoaDonRequest
    {
        public int? IMaKh { get; set; }
        public decimal FGiamGia { get; set; } = 0;
        public string SPhuongThucTt { get; set; } = "Tien mat";
        public decimal FTienKhachTra { get; set; } = 0;
        public List<HoaDonLineDto> Lines { get; set; } = new();
    }

    public class HoaDonLineDto
    {
        public int IMaSp { get; set; }
        public int IMaSize { get; set; }
        public int IMaMau { get; set; }
        public int ISoLuong { get; set; }
        public decimal FDonGia { get; set; }
    }
}
