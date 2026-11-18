using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers.Api;

[ApiController]
[Route("api/sanpham")]
[Authorize]
public class SanPhamApiController : ControllerBase
{
    private readonly QuanLyCuaHangTheThaoContext _db;

    public SanPhamApiController(QuanLyCuaHangTheThaoContext db)
    {
        _db = db;
    }

    // GET /api/sanpham?search=...&page=1&pageSize=20
    [HttpGet]
    public async Task<IActionResult> Get(string? search = null, int page = 1, int pageSize = 20)
    {
        pageSize = Math.Clamp(pageSize, 1, 100);
        page = Math.Max(1, page);

        var query = _db.TblSanPhams.Include(s => s.IMaNccNavigation).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(s => s.STenSp.Contains(search) || s.IMaNccNavigation.STenNcc.Contains(search));

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(s => s.IMaSp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new
            {
                s.IMaSp,
                s.STenSp,
                s.IMaNcc,
                ncc = s.IMaNccNavigation.STenNcc,
                s.SXuatXu,
                s.FGiaNhap,
                s.FGiaBan,
                s.SMoTa,
                s.ISoLuongTon,
                hanDung = s.DNgayHetHan.HasValue ? s.DNgayHetHan.Value.ToString("yyyy-MM-dd") : null
            })
            .ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }

    // GET /api/sanpham/{id} - chi tiet kem size, mau, ton kho theo bien the
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var sp = await _db.TblSanPhams
            .Include(s => s.TblSanPhamTblKichThuocs).ThenInclude(j => j.IMaSizeNavigation)
            .Include(s => s.TblSanPhamTblMauSacs).ThenInclude(j => j.IMaMauNavigation)
            .FirstOrDefaultAsync(s => s.IMaSp == id);
        if (sp == null) return NotFound();

        var tonKho = await _db.TblTonKhoChiTiets
            .Where(t => t.IMaSp == id)
            .Select(t => new { t.IMaSize, t.IMaMau, t.ISoLuongTon })
            .ToListAsync();

        var result = new
        {
            sp.IMaSp,
            sp.STenSp,
            sp.IMaNcc,
            sp.SXuatXu,
            sp.FGiaNhap,
            sp.FGiaBan,
            sp.SMoTa,
            sp.ISoLuongTon,
            sizes = sp.TblSanPhamTblKichThuocs.Select(j => new { j.IMaSize, j.IMaSizeNavigation.STenSize }),
            maus = sp.TblSanPhamTblMauSacs.Select(j => new { j.IMaMau, j.IMaMauNavigation.STenMau }),
            tonKho
        };
        return Ok(result);
    }
}
