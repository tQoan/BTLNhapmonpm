using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers.Api;

[ApiController]
[Route("api/khachhang")]
[Authorize]
public class KhachHangApiController : ControllerBase
{
    private readonly QuanLyCuaHangTheThaoContext _db;

    public KhachHangApiController(QuanLyCuaHangTheThaoContext db)
    {
        _db = db;
    }

    // GET /api/khachhang?search=...
    [HttpGet]
    public async Task<IActionResult> Get(string? search = null)
    {
        var query = _db.TblKhachHangs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.STenKh.Contains(search)
                || (x.SDienThoai != null && x.SDienThoai.Contains(search)));

        var items = await query
            .OrderBy(x => x.STenKh)
            .Select(x => new { x.IMaKh, x.STenKh, x.SDienThoai, x.SDiaChi })
            .ToListAsync();
        return Ok(items);
    }

    // POST /api/khachhang
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] KhachHangDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.STenKh))
            return BadRequest(new { message = "Tên khách hàng không được để trống." });
        if (!string.IsNullOrWhiteSpace(dto.SDienThoai)
            && await _db.TblKhachHangs.AnyAsync(x => x.SDienThoai == dto.SDienThoai.Trim()))
            return Conflict(new { message = "Số điện thoại đã tồn tại." });

        var entity = new TblKhachHang
        {
            STenKh = dto.STenKh.Trim(),
            SDienThoai = string.IsNullOrWhiteSpace(dto.SDienThoai) ? null : dto.SDienThoai.Trim(),
            SDiaChi = dto.SDiaChi
        };
        _db.TblKhachHangs.Add(entity);
        await _db.SaveChangesAsync();
        return Created($"/api/khachhang/{entity.IMaKh}", new { entity.IMaKh, entity.STenKh, entity.SDienThoai, entity.SDiaChi });
    }

    public class KhachHangDto
    {
        public string STenKh { get; set; } = string.Empty;
        public string? SDienThoai { get; set; }
        public string? SDiaChi { get; set; }
    }
}
