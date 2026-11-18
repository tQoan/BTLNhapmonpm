using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers.Api;

[ApiController]
[Route("api/tonkho")]
[Authorize]
public class TonKhoApiController : ControllerBase
{
    private readonly QuanLyCuaHangTheThaoContext _db;

    public TonKhoApiController(QuanLyCuaHangTheThaoContext db)
    {
        _db = db;
    }

    // GET /api/tonkho?sapHet=true
    [HttpGet]
    public async Task<IActionResult> Get(bool? sapHet = null, string? search = null)
    {
        var query = _db.TblTonKhoChiTiets
            .Include(t => t.TblSanPhamTblKichThuoc).ThenInclude(j => j.IMaSpNavigation)
            .Include(t => t.TblSanPhamTblKichThuoc).ThenInclude(j => j.IMaSizeNavigation)
            .Include(t => t.TblSanPhamTblMauSac).ThenInclude(j => j.IMaMauNavigation)
            .AsQueryable();

        if (sapHet == true)
            query = query.Where(t => t.ISoLuongTon <= 5);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.TblSanPhamTblKichThuoc.IMaSpNavigation.STenSp.Contains(search));

        var items = await query
            .Select(t => new
            {
                t.IMaSp,
                tenSp = t.TblSanPhamTblKichThuoc.IMaSpNavigation.STenSp,
                t.IMaSize,
                tenSize = t.TblSanPhamTblKichThuoc.IMaSizeNavigation.STenSize,
                t.IMaMau,
                tenMau = t.TblSanPhamTblMauSac.IMaMauNavigation.STenMau,
                t.ISoLuongTon
            })
            .OrderBy(x => x.tenSp)
            .ToListAsync();

        return Ok(items);
    }
}
