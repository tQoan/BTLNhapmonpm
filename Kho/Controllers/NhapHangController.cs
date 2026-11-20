using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers;

[Authorize]
public class NhapHangController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;
    private readonly ILogger<NhapHangController> _logger;

    public NhapHangController(QuanLyCuaHangTheThaoContext db, ILogger<NhapHangController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search = null, int page = 1)
    {
        const int pageSize = 10;
        var query = _db.TblPhieuNhaps
            .Include(p => p.IMaNccNavigation)
            .Include(p => p.IMaNvNavigation).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.IMaPn.ToString().Contains(search) || p.IMaNccNavigation.STenNcc.Contains(search));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.IMaPn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.TongPhieu = total;
        ViewBag.TongChiTiet = await _db.TblChiTietPhieuNhaps.CountAsync();
        ViewBag.TongGiaTri = await _db.TblChiTietPhieuNhaps.SumAsync(l => l.FDonGiaNhap * l.ISoLuong);
        return View(new PaginatedList<TblPhieuNhap>(items, total, page, pageSize));
    }

    public async Task<IActionResult> Create()
    {
        await PopulateCreateData();
        return View(new PhieuNhapViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PhieuNhapViewModel model)
    {
        await PopulateCreateData();

        var validLines = model.Lines.Where(l => l.IMaSp > 0 && l.ISoLuong > 0).ToList();
        if (model.IMaNcc <= 0)
            ModelState.AddModelError(string.Empty, "Vui lòng chọn nhà cung cấp.");
        if (!validLines.Any())
            ModelState.AddModelError(string.Empty, "Vui lòng thêm ít nhất một dòng chi tiết nhập hàng.");

        if (!ModelState.IsValid) return View(model);

        int maNV = int.Parse(User.FindFirstValue("iMaNV") ?? "0");

        var phieu = new TblPhieuNhap
        {
            DNgayNhap = model.DNgayNhap,
            IMaNcc = model.IMaNcc,
            IMaNv = maNV
        };
        _db.TblPhieuNhaps.Add(phieu);
        await _db.SaveChangesAsync();

        foreach (var line in validLines)
        {
            _db.TblChiTietPhieuNhaps.Add(new TblChiTietPhieuNhap
            {
                IMaPn = phieu.IMaPn,
                IMaSp = line.IMaSp,
                IMaSize = line.IMaSize,
                IMaMau = line.IMaMau,
                ISoLuong = line.ISoLuong,
                FDonGiaNhap = line.FDonGiaNhap
            });
        }

        try
        {
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Đã lưu phiếu nhập #{phieu.IMaPn}.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            _db.TblPhieuNhaps.Remove(phieu);
            await _db.SaveChangesAsync();
            ModelState.AddModelError(string.Empty, "Lưu phiếu nhập thất bại, vui lòng kiểm tra lại dữ liệu.");
            return View(model);
        }
    }

    private async Task PopulateCreateData()
    {
        ViewBag.NhaCungCaps = await _db.TblNhaCungCaps.OrderBy(x => x.STenNcc).ToListAsync();
        ViewBag.SanPhams = await _db.TblSanPhams.OrderBy(x => x.STenSp).ToListAsync();

        // variant map: product -> {maus:[{id,name}], sizes:[{id,name}]}
        var sizeMap = await _db.TblSanPhamTblKichThuocs.ToListAsync();
        var mauMap = await _db.TblSanPhamTblMauSacs.ToListAsync();
        var sizes = await _db.TblKichThuocs.ToDictionaryAsync(x => x.IMaSize);
        var maus = await _db.TblMauSacs.ToDictionaryAsync(x => x.IMaMau);

        var variants = new Dictionary<int, object>();
        foreach (var sp in await _db.TblSanPhams.ToListAsync())
        {
            var spSizes = sizeMap.Where(x => x.IMaSp == sp.IMaSp)
                .Select(x => new { id = x.IMaSize, name = sizes[x.IMaSize].STenSize }).ToList();
            var spMaus = mauMap.Where(x => x.IMaSp == sp.IMaSp)
                .Select(x => new { id = x.IMaMau, name = maus[x.IMaMau].STenMau }).ToList();
            variants[sp.IMaSp] = new { sizes = spSizes, maus = spMaus };
        }
        ViewBag.VariantsJson = System.Text.Json.JsonSerializer.Serialize(variants);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var phieu = await _db.TblPhieuNhaps
            .Include(p => p.IMaNccNavigation)
            .Include(p => p.IMaNvNavigation)
            .FirstOrDefaultAsync(p => p.IMaPn == id);
        if (phieu == null) return NotFound();

        var lines = await _db.TblChiTietPhieuNhaps
            .Where(c => c.IMaPn == id)
            .Include(c => c.TblSanPhamTblKichThuoc).ThenInclude(j => j.IMaSizeNavigation)
            .Include(c => c.TblSanPhamTblKichThuoc).ThenInclude(j => j.IMaSpNavigation)
            .Include(c => c.TblSanPhamTblMauSac).ThenInclude(j => j.IMaMauNavigation)
            .ToListAsync();

        ViewBag.Phieu = phieu;
        return View(lines);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var phieu = await _db.TblPhieuNhaps.FindAsync(id);
        if (phieu == null) return NotFound();

        var lines = await _db.TblChiTietPhieuNhaps
            .Where(c => c.IMaPn == id)
            .ToListAsync();

        var model = new PhieuNhapViewModel
        {
            IMaNcc = phieu.IMaNcc,
            DNgayNhap = phieu.DNgayNhap,
            Lines = lines.Select(l => new PhieuNhapLine
            {
                IMaSp = l.IMaSp,
                IMaSize = l.IMaSize,
                IMaMau = l.IMaMau,
                ISoLuong = l.ISoLuong,
                FDonGiaNhap = l.FDonGiaNhap
            }).ToList()
        };

        ViewBag.IMaPn = id;
        await PopulateCreateData();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PhieuNhapViewModel model)
    {
        await PopulateCreateData();
        ViewBag.IMaPn = id;

        var validLines = model.Lines.Where(l => l.IMaSp > 0 && l.ISoLuong > 0).ToList();
        if (model.IMaNcc <= 0)
            ModelState.AddModelError(string.Empty, "Vui lòng chọn nhà cung cấp.");
        if (!validLines.Any())
            ModelState.AddModelError(string.Empty, "Vui lòng thêm ít nhất một dòng chi tiết nhập hàng.");

        if (!ModelState.IsValid) return View(model);

        var phieu = await _db.TblPhieuNhaps.FindAsync(id);
        if (phieu == null) return NotFound();

        phieu.IMaNcc = model.IMaNcc;
        phieu.DNgayNhap = model.DNgayNhap;

        var oldLines = await _db.TblChiTietPhieuNhaps.Where(c => c.IMaPn == id).ToListAsync();

        try
        {
            // Undo stock that was previously added for each OLD line (the INSERT trigger
            // will re-add stock for the new lines after we replace them).
            foreach (var old in oldLines)
            {
                var tk = await _db.TblTonKhoChiTiets
                    .FirstOrDefaultAsync(t => t.IMaSp == old.IMaSp && t.IMaSize == old.IMaSize && t.IMaMau == old.IMaMau);
                if (tk != null)
                    tk.ISoLuongTon -= old.ISoLuong;
            }

            // Replace lines: remove old, re-add valid (trigger fires for new adds)
            _db.TblChiTietPhieuNhaps.RemoveRange(oldLines);
            foreach (var line in validLines)
            {
                _db.TblChiTietPhieuNhaps.Add(new TblChiTietPhieuNhap
                {
                    IMaPn = id,
                    IMaSp = line.IMaSp,
                    IMaSize = line.IMaSize,
                    IMaMau = line.IMaMau,
                    ISoLuong = line.ISoLuong,
                    FDonGiaNhap = line.FDonGiaNhap
                });
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = $"Đã cập nhật phiếu nhập #{id}.";
            return RedirectToAction("Detail", new { id });
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "Cập nhật phiếu nhập thất bại, vui lòng kiểm tra lại dữ liệu.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var phieu = await _db.TblPhieuNhaps.FindAsync(id);
            if (phieu == null) return NotFound();

            var lines = await _db.TblChiTietPhieuNhaps
                .Where(c => c.IMaPn == id)
                .ToListAsync();

            // Tru bot ton kho da cong khi nhap (undo trigger INSERT)
            foreach (var line in lines)
            {
                var tk = await _db.TblTonKhoChiTiets
                    .FirstOrDefaultAsync(t => t.IMaSp == line.IMaSp && t.IMaSize == line.IMaSize && t.IMaMau == line.IMaMau);
                if (tk != null)
                    tk.ISoLuongTon = Math.Max(0, tk.ISoLuongTon - line.ISoLuong);
            }

            _db.TblChiTietPhieuNhaps.RemoveRange(lines);
            await _db.SaveChangesAsync();

            _db.TblPhieuNhaps.Remove(phieu);
            await _db.SaveChangesAsync();

            _logger.LogWarning("Phieu nhap #{MaPN} da bi xoa (hoan tra ton kho)", id);
            TempData["Success"] = $"Đã xóa phiếu nhập #{id} và điều chỉnh tồn kho.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            _logger.LogError("Khong the xoa phieu nhap #{MaPN} do rang buoc du lieu", id);
            TempData["Error"] = "Không thể xóa phiếu nhập do ràng buộc dữ liệu liên quan.";
            return RedirectToAction(nameof(Index));
        }
    }
}
