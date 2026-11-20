using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers;

[Authorize]
public class SanPhamController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<SanPhamController> _logger;

    public SanPhamController(QuanLyCuaHangTheThaoContext db, ILogger<SanPhamController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search = null, int page = 1)
    {
        const int pageSize = 10;
        var query = _db.TblSanPhams.Include(s => s.IMaNccNavigation).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.STenSp.Contains(search) || s.IMaNccNavigation.STenNcc.Contains(search));

        var total = await query.CountAsync();
        var filtered = await query.ToListAsync();

        var items = filtered
            .OrderByDescending(s => s.IMaSp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var model = new PaginatedList<TblSanPham>(items, total, page, pageSize);

        ViewBag.Search = search;
        ViewBag.TongSanPham = total;
        ViewBag.TongTonKho = filtered.Sum(s => s.ISoLuongTon);
        ViewBag.TongGiaTriTon = filtered.Sum(s => s.ISoLuongTon * s.FGiaNhap);
        ViewBag.TongGiaTriBan = filtered.Sum(s => s.ISoLuongTon * s.FGiaBan);
        ViewBag.HetHang = filtered.Count(s => s.ISoLuongTon == 0);
        return View(model);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateLookups();
        return View(new SanPhamViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SanPhamViewModel model)
    {
        if (model.FGiaBan < model.FGiaNhap)
            ModelState.AddModelError(nameof(SanPhamViewModel.FGiaBan), "Giá bán phải lớn hơn hoặc bằng giá nhập.");

        if (!model.SelectedSizes.Any() || !model.SelectedMaus.Any())
            ModelState.AddModelError(string.Empty, "Vui lòng chọn ít nhất một kích thước và một màu sắc.");

        if (!ModelState.IsValid)
        {
            await PopulateLookups(model);
            return View(model);
        }

        var sp = new TblSanPham
        {
            STenSp = model.STenSp,
            IMaNcc = model.IMaNcc,
            SXuatXu = model.SXuatXu,
            FGiaNhap = model.FGiaNhap,
            FGiaBan = model.FGiaBan,
            SMoTa = model.SMoTa,
            DNgayHetHan = model.DNgayHetHan.HasValue ? DateOnly.FromDateTime(model.DNgayHetHan.Value) : null,
            ISoLuongTon = 0
        };
        _db.TblSanPhams.Add(sp);
        await _db.SaveChangesAsync();

        // Subtype
        switch (model.LoaiSanPham)
        {
            case "quan_ao":
                _db.TblQuanAos.Add(new TblQuanAo { IMaSp = sp.IMaSp, SChatLieu = model.SChatLieu, SKieuDang = model.SKieuDang });
                break;
            case "giay_dep":
                _db.TblGiayDeps.Add(new TblGiayDep { IMaSp = sp.IMaSp, SChatLieu = model.SChatLieu, SLoaiDe = model.SLoaiDe });
                break;
            case "dung_cu":
                _db.TblDungCus.Add(new TblDungCu { IMaSp = sp.IMaSp, SChatLieu = model.SChatLieu, SCongDung = model.SCongDung });
                break;
        }

        // Sizes
        foreach (var sizeId in model.SelectedSizes)
            _db.TblSanPhamTblKichThuocs.Add(new TblSanPhamTblKichThuoc { IMaSp = sp.IMaSp, IMaSize = sizeId });

        // Maus
        foreach (var mauId in model.SelectedMaus)
            _db.TblSanPhamTblMauSacs.Add(new TblSanPhamTblMauSac { IMaSp = sp.IMaSp, IMaMau = mauId });

        // Seed ton kho for each size x mau combo (required by FK + triggers)
        foreach (var sizeId in model.SelectedSizes)
            foreach (var mauId in model.SelectedMaus)
                _db.TblTonKhoChiTiets.Add(new TblTonKhoChiTiet { IMaSp = sp.IMaSp, IMaSize = sizeId, IMaMau = mauId, ISoLuongTon = 0 });

        await _db.SaveChangesAsync();

        SaveImage(model.HinhAnh, sp.IMaSp);

        TempData["Success"] = "Đã thêm sản phẩm mới.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var sp = await _db.TblSanPhams
            .Include(s => s.TblSanPhamTblKichThuocs)
            .Include(s => s.TblSanPhamTblMauSacs)
            .Include(s => s.TblQuanAo)
            .Include(s => s.TblGiayDep)
            .Include(s => s.TblDungCu)
            .FirstOrDefaultAsync(s => s.IMaSp == id);
        if (sp == null) return NotFound();

        var model = new SanPhamViewModel
        {
            IMaSp = sp.IMaSp,
            STenSp = sp.STenSp,
            IMaNcc = sp.IMaNcc,
            SXuatXu = sp.SXuatXu,
            FGiaNhap = sp.FGiaNhap,
            FGiaBan = sp.FGiaBan,
            SMoTa = sp.SMoTa,
            DNgayHetHan = sp.DNgayHetHan.HasValue ? sp.DNgayHetHan.Value.ToDateTime(TimeOnly.MinValue) : null,
            SelectedSizes = sp.TblSanPhamTblKichThuocs.Select(x => x.IMaSize).ToList(),
            SelectedMaus = sp.TblSanPhamTblMauSacs.Select(x => x.IMaMau).ToList()
        };

        if (sp.TblQuanAo != null) { model.LoaiSanPham = "quan_ao"; model.SChatLieu = sp.TblQuanAo.SChatLieu; model.SKieuDang = sp.TblQuanAo.SKieuDang; }
        else if (sp.TblGiayDep != null) { model.LoaiSanPham = "giay_dep"; model.SChatLieu = sp.TblGiayDep.SChatLieu; model.SLoaiDe = sp.TblGiayDep.SLoaiDe; }
        else if (sp.TblDungCu != null) { model.LoaiSanPham = "dung_cu"; model.SChatLieu = sp.TblDungCu.SChatLieu; model.SCongDung = sp.TblDungCu.SCongDung; }

        await PopulateLookups(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SanPhamViewModel model)
    {
        if (model.FGiaBan < model.FGiaNhap)
            ModelState.AddModelError(nameof(SanPhamViewModel.FGiaBan), "Giá bán phải lớn hơn hoặc bằng giá nhập.");

        if (!model.SelectedSizes.Any() || !model.SelectedMaus.Any())
            ModelState.AddModelError(string.Empty, "Vui lòng chọn ít nhất một kích thước và một màu sắc.");

        if (!ModelState.IsValid)
        {
            await PopulateLookups(model);
            return View(model);
        }

        var sp = await _db.TblSanPhams
            .Include(s => s.TblSanPhamTblKichThuocs)
            .Include(s => s.TblSanPhamTblMauSacs)
            .Include(s => s.TblQuanAo)
            .Include(s => s.TblGiayDep)
            .Include(s => s.TblDungCu)
            .FirstOrDefaultAsync(s => s.IMaSp == model.IMaSp);
        if (sp == null) return NotFound();

        sp.STenSp = model.STenSp;
        sp.IMaNcc = model.IMaNcc;
        sp.SXuatXu = model.SXuatXu;
        sp.FGiaNhap = model.FGiaNhap;
        sp.FGiaBan = model.FGiaBan;
        sp.SMoTa = model.SMoTa;
        sp.DNgayHetHan = model.DNgayHetHan.HasValue ? DateOnly.FromDateTime(model.DNgayHetHan.Value) : null;

        // Update subtype
        _db.TblQuanAos.RemoveRange(sp.TblQuanAo == null ? Enumerable.Empty<TblQuanAo>() : new[] { sp.TblQuanAo });
        _db.TblGiayDeps.RemoveRange(sp.TblGiayDep == null ? Enumerable.Empty<TblGiayDep>() : new[] { sp.TblGiayDep });
        _db.TblDungCus.RemoveRange(sp.TblDungCu == null ? Enumerable.Empty<TblDungCu>() : new[] { sp.TblDungCu });
        switch (model.LoaiSanPham)
        {
            case "quan_ao": _db.TblQuanAos.Add(new TblQuanAo { IMaSp = sp.IMaSp, SChatLieu = model.SChatLieu, SKieuDang = model.SKieuDang }); break;
            case "giay_dep": _db.TblGiayDeps.Add(new TblGiayDep { IMaSp = sp.IMaSp, SChatLieu = model.SChatLieu, SLoaiDe = model.SLoaiDe }); break;
            case "dung_cu": _db.TblDungCus.Add(new TblDungCu { IMaSp = sp.IMaSp, SChatLieu = model.SChatLieu, SCongDung = model.SCongDung }); break;
        }

        // Sizes
        var oldSizes = sp.TblSanPhamTblKichThuocs.Select(x => x.IMaSize).ToList();
        _db.TblSanPhamTblKichThuocs.RemoveRange(sp.TblSanPhamTblKichThuocs);
        foreach (var sizeId in model.SelectedSizes)
            _db.TblSanPhamTblKichThuocs.Add(new TblSanPhamTblKichThuoc { IMaSp = sp.IMaSp, IMaSize = sizeId });

        // Maus
        var oldMaus = sp.TblSanPhamTblMauSacs.Select(x => x.IMaMau).ToList();
        _db.TblSanPhamTblMauSacs.RemoveRange(sp.TblSanPhamTblMauSacs);
        foreach (var mauId in model.SelectedMaus)
            _db.TblSanPhamTblMauSacs.Add(new TblSanPhamTblMauSac { IMaSp = sp.IMaSp, IMaMau = mauId });

        await _db.SaveChangesAsync();

        // Sync ton kho rows (add newly needed combos)
        var combos = new HashSet<(int, int)>(_db.TblTonKhoChiTiets
            .Where(t => t.IMaSp == sp.IMaSp)
            .Select(t => new ValueTuple<int, int>(t.IMaSize, t.IMaMau))
            .ToList());

        foreach (var sizeId in model.SelectedSizes)
            foreach (var mauId in model.SelectedMaus)
                if (!combos.Contains((sizeId, mauId)))
                    _db.TblTonKhoChiTiets.Add(new TblTonKhoChiTiet { IMaSp = sp.IMaSp, IMaSize = sizeId, IMaMau = mauId, ISoLuongTon = 0 });

        await _db.SaveChangesAsync();

        SaveImage(model.HinhAnh, sp.IMaSp, removeIfNone: true);

        TempData["Success"] = "Đã cập nhật sản phẩm.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var sp = await _db.TblSanPhams.FindAsync(id);
            if (sp == null) return NotFound();

            var hasTransactions = await _db.TblChiTietHoaDons.AnyAsync(c => c.IMaSp == id)
                || await _db.TblChiTietPhieuNhaps.AnyAsync(c => c.IMaSp == id);
            if (hasTransactions)
            {
                TempData["Error"] = "Không thể xóa sản phẩm đã phát sinh giao dịch (nhập/bán).";
                return RedirectToAction(nameof(Index));
            }

            // Remove ton kho detail rows first (FK NO ACTION prevents cascade)
            var tonKho = await _db.TblTonKhoChiTiets.Where(t => t.IMaSp == id).ToListAsync();
            _db.TblTonKhoChiTiets.RemoveRange(tonKho);
            await _db.SaveChangesAsync();

            _db.TblSanPhams.Remove(sp);
            await _db.SaveChangesAsync();

            foreach (var ext in _allowedExts)
            {
                var p = ImagePath(id) + ext;
                if (System.IO.File.Exists(p)) System.IO.File.Delete(p);
            }

            TempData["Success"] = "Đã xóa sản phẩm.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Không thể xóa sản phẩm do ràng buộc dữ liệu liên quan.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLookups(SanPhamViewModel? model = null)
    {
        ViewBag.NhaCungCaps = await _db.TblNhaCungCaps.OrderBy(x => x.STenNcc).ToListAsync();
        ViewBag.KichThuocs = await _db.TblKichThuocs.OrderBy(x => x.IMaSize).ToListAsync();
        ViewBag.MauSacs = await _db.TblMauSacs.OrderBy(x => x.IMaMau).ToListAsync();
        ViewBag.Model = model;
    }

    private static readonly string[] _allowedExts = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private static string ImagePath(int id) =>
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", $"{id}");

    private static void SaveImage(IFormFile? file, int id, bool removeIfNone = false)
    {
        var dir = Path.GetDirectoryName(ImagePath(id));
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir!);

        // Remove old images of any allowed extension
        foreach (var ext in _allowedExts)
        {
            var old = ImagePath(id) + ext;
            if (System.IO.File.Exists(old)) System.IO.File.Delete(old);
        }

        if (file == null || file.Length == 0)
        {
            if (removeIfNone) return;
            return;
        }

        var fileExt = Path.GetExtension(file.FileName).ToLowerInvariant();
        var target = ImagePath(id) + fileExt;
        using (var stream = new FileStream(target, FileMode.Create))
        {
            file.CopyTo(stream);
        }
    }

    public static string? GetImageUrl(int id)
    {
        foreach (var ext in _allowedExts)
        {
            if (System.IO.File.Exists(ImagePath(id) + ext))
                return $"/images/products/{id}{ext}";
        }
        return null;
    }
}
