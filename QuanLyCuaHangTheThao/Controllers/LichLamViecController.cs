using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models;
using QuanLyCuaHangTheThao.Models.Scaffold;
using QuanLyCuaHangTheThao.Services;

namespace QuanLyCuaHangTheThao.Controllers;

/// <summary>
/// Phan cong ca theo tuan (T2 -> CN). Admin quan ly toan bo, nhan vien chi xem lich cua minh.
/// </summary>
[Authorize]
public class LichLamViecController : Controller
{
    private const int SoNgayXemTruoc = 14;

    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<LichLamViecController> _logger;

    public LichLamViecController(QuanLyCuaHangTheThaoContext db, ILogger<LichLamViecController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index(DateTime? tuan = null)
    {
        var dauTuan = LichLamViecTinh.DauTuan(tuan ?? DateTime.Today);
        var ngayTrongTuan = LichLamViecTinh.NgayTrongTuan(dauTuan);
        var hetTuan = ngayTrongTuan[^1];
        var laAdmin = User.IsInRole("Admin");

        var danhSachCa = await _db.TblCaLamViecs
            .OrderBy(c => c.DGioBatDau)
            .ThenBy(c => c.IMaCa)
            .ToListAsync();

        var nhanVienQuery = _db.TblNhanViens
            .Include(n => n.IMaVaiTroNavigation)
            .OrderBy(n => n.STenNv)
            .AsQueryable();

        // Nhan vien khong phai Admin chi duoc xem lich cua chinh minh.
        if (!laAdmin)
        {
            var maNv = MaNhanVienHienTai();
            nhanVienQuery = maNv.HasValue
                ? nhanVienQuery.Where(n => n.IMaNv == maNv.Value)
                : nhanVienQuery.Where(n => false);
        }

        var danhSachNv = await nhanVienQuery.ToListAsync();

        var lichTheoTuan = await _db.TblLichLamViecs
            .Include(l => l.IMaCaNavigation)
            .Where(l => l.DNgayLam >= dauTuan && l.DNgayLam <= hetTuan)
            .ToListAsync();

        var model = new LichLamViecViewModel
        {
            DauTuan = dauTuan,
            DanhSachCa = danhSachCa
        };

        foreach (var nv in danhSachNv)
        {
            var hang = new HangLichLamViec
            {
                IMaNv = nv.IMaNv,
                STenNv = nv.STenNv,
                SDienThoai = nv.SDienThoai,
                SVaiTro = nv.IMaVaiTroNavigation?.STenVaiTro,
                Os = new LichLamViecO[LichLamViecTinh.SoNgayTuan]
            };

            for (int i = 0; i < LichLamViecTinh.SoNgayTuan; i++)
            {
                var lich = lichTheoTuan.FirstOrDefault(l => l.IMaNv == nv.IMaNv && l.DNgayLam == ngayTrongTuan[i]);
                hang.Os[i] = new LichLamViecO { IMaLich = lich?.IMaLich, IMaCa = lich?.IMaCa };
            }

            model.Luoi.Add(hang);
        }

        ViewBag.NgayTrongTuan = ngayTrongTuan;
        ViewBag.KhoangTuan = LichLamViecTinh.FormatKhoangTuan(dauTuan);
        ViewBag.TuanTruoc = LichLamViecTinh.FormatKhoangTuan(dauTuan.AddDays(-LichLamViecTinh.SoNgayTuan));
        ViewBag.TuanSau = LichLamViecTinh.FormatKhoangTuan(dauTuan.AddDays(LichLamViecTinh.SoNgayTuan));
        ViewBag.LaAdmin = laAdmin;
        ViewBag.TongCaTrongTuan = lichTheoTuan.Count;
        ViewBag.TongGioTrongTuan = Math.Round(lichTheoTuan.Sum(l =>
            LichLamViecTinh.SoGioCa(l.IMaCaNavigation.DGioBatDau, l.IMaCaNavigation.DGioKetThuc)), 1);
        ViewBag.TongNv = danhSachNv.Count;
        ViewBag.NvCoLich = lichTheoTuan.Select(l => l.IMaNv).Distinct().Count();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Save(LichLamViecViewModel model)
    {
        var dauTuan = LichLamViecTinh.DauTuan(model.DauTuan == default ? DateTime.Today : model.DauTuan);
        var ngayTrongTuan = LichLamViecTinh.NgayTrongTuan(dauTuan);
        var ngayDau = ngayTrongTuan[0];
        var ngayCuoi = ngayTrongTuan[LichLamViecTinh.SoNgayTuan - 1];

        var maCasHopLe = (await _db.TblCaLamViecs.Select(c => c.IMaCa).ToListAsync()).ToHashSet();
        var maNvsHopLe = await _db.TblNhanViens.Select(n => n.IMaNv).ToListAsync();

        var lichDaCo = await _db.TblLichLamViecs
            .Where(l => l.DNgayLam >= ngayDau && l.DNgayLam <= ngayCuoi)
            .ToListAsync();

        int them = 0, sua = 0, xoa = 0, bo = 0;

        foreach (var hang in model.Luoi ?? new List<HangLichLamViec>())
        {
            if (!maNvsHopLe.Contains(hang.IMaNv))
            {
                bo += hang.Os?.Length ?? 0;
                continue;
            }

            var os = hang.Os ?? new LichLamViecO[LichLamViecTinh.SoNgayTuan];

            for (int i = 0; i < LichLamViecTinh.SoNgayTuan; i++)
            {
                var o = os[i];
                if (o == null) continue;

                var ngay = ngayTrongTuan[i];
                var lichHienTai = lichDaCo.FirstOrDefault(l => l.IMaNv == hang.IMaNv && l.DNgayLam == ngay);
                var maCa = o.IMaCa;

                if (maCa.HasValue && maCasHopLe.Contains(maCa.Value))
                {
                    if (lichHienTai == null)
                    {
                        _db.TblLichLamViecs.Add(new TblLichLamViec
                        {
                            IMaNv = hang.IMaNv,
                            IMaCa = maCa.Value,
                            DNgayLam = ngay
                        });
                        them++;
                    }
                    else if (lichHienTai.IMaCa != maCa.Value)
                    {
                        lichHienTai.IMaCa = maCa.Value;
                        sua++;
                    }
                }
                else if (lichHienTai != null)
                {
                    _db.TblLichLamViecs.Remove(lichHienTai);
                    lichDaCo.Remove(lichHienTai);
                    xoa++;
                }
            }
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation(
            "Luu lich lam viec tuan {DauTuan:yyyy-MM-dd}: them {Them}, sua {Sua}, xoa {Xoa}, bo {Bo}",
            dauTuan, them, sua, xoa, bo);

        TempData["Success"] = bo > 0
            ? $"Đã lưu lịch tuần {LichLamViecTinh.FormatKhoangTuan(dauTuan)} (thêm {them}, sửa {sua}, xóa {xoa}, bỏ {bo} dòng không hợp lệ)."
            : $"Đã lưu lịch tuần {LichLamViecTinh.FormatKhoangTuan(dauTuan)} (thêm {them}, sửa {sua}, xóa {xoa}).";

        return RedirectToAction(nameof(Index), new { tuan = dauTuan.ToString("yyyy-MM-dd") });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CopyTuan(DateTime? tuan = null)
    {
        var dauTuan = LichLamViecTinh.DauTuan(tuan ?? DateTime.Today);
        var ngayTrongTuan = LichLamViecTinh.NgayTrongTuan(dauTuan);
        var ngayTuanTruoc = LichLamViecTinh.NgayTrongTuan(dauTuan.AddDays(-LichLamViecTinh.SoNgayTuan));

        var ngayDauTuanTruoc = ngayTuanTruoc[0];
        var ngayCuoiTuanTruoc = ngayTuanTruoc[LichLamViecTinh.SoNgayTuan - 1];
        var ngayDauTuanDich = ngayTrongTuan[0];
        var ngayCuoiTuanDich = ngayTrongTuan[LichLamViecTinh.SoNgayTuan - 1];

        var lichTuanTruoc = await _db.TblLichLamViecs
            .Where(l => l.DNgayLam >= ngayDauTuanTruoc && l.DNgayLam <= ngayCuoiTuanTruoc)
            .ToListAsync();

        if (lichTuanTruoc.Count == 0)
        {
            TempData["Error"] = $"Tuần trước không có ca nào để sao chép.";
            return RedirectToAction(nameof(Index), new { tuan = dauTuan.ToString("yyyy-MM-dd") });
        }

        var lichTuanDich = await _db.TblLichLamViecs
            .Where(l => l.DNgayLam >= ngayDauTuanDich && l.DNgayLam <= ngayCuoiTuanDich)
            .ToListAsync();

        int saoChep = 0, boQua = 0;

        foreach (var nguon in lichTuanTruoc)
        {
            int dayIndex = Array.IndexOf(ngayTuanTruoc, nguon.DNgayLam);
            if (dayIndex < 0) continue;

            var ngayDich = ngayTrongTuan[dayIndex];
            var dich = lichTuanDich.FirstOrDefault(l => l.IMaNv == nguon.IMaNv && l.DNgayLam == ngayDich);
            if (dich != null)
            {
                boQua++;
                continue;
            }

            _db.TblLichLamViecs.Add(new TblLichLamViec
            {
                IMaNv = nguon.IMaNv,
                IMaCa = nguon.IMaCa,
                DNgayLam = ngayDich,
                SGhiChu = nguon.SGhiChu
            });
            saoChep++;
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Sao chep {SoChep} dong lich tu {DauTuan:yyyy-MM-dd}", saoChep, dauTuan);

        TempData["Success"] = boQua > 0
            ? $"Đã sao chép {saoChep} ca từ tuần trước, bỏ qua {boQua} nhân viên đã có ca trong tuần này."
            : $"Đã sao chép {saoChep} ca từ tuần trước.";

        return RedirectToAction(nameof(Index), new { tuan = dauTuan.ToString("yyyy-MM-dd") });
    }

    /// <summary>Lich ca sap toi cua nguoi dang dang nhap.</summary>
    public async Task<IActionResult> CaSapToi()
    {
        var maNv = MaNhanVienHienTai();
        if (!maNv.HasValue)
        {
            _logger.LogWarning("Khong tim thay claim iMaNV cho nguoi dung {User}", User.Identity?.Name);
            return RedirectToAction(nameof(Index));
        }

        var homNay = DateTime.Today;
        var denNgay = homNay.AddDays(SoNgayXemTruoc - 1);

        var caSapToi = await _db.TblLichLamViecs
            .Include(l => l.IMaCaNavigation)
            .Where(l => l.IMaNv == maNv.Value && l.DNgayLam >= homNay && l.DNgayLam <= denNgay)
            .OrderBy(l => l.DNgayLam)
            .ToListAsync();

        ViewBag.TenNv = User.Identity?.Name;
        ViewBag.DenNgay = denNgay;
        ViewBag.KhoangTuan = LichLamViecTinh.FormatKhoangTuan(homNay);
        return View(caSapToi);
    }

    private int? MaNhanVienHienTai()
    {
        var raw = User.FindFirst("iMaNV")?.Value;
        return int.TryParse(raw, out var ma) ? ma : null;
    }
}