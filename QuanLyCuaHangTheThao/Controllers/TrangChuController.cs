using Microsoft.AspNetCore.Mvc;
using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Controllers;

public class TrangChuController : Controller
{
    private readonly QuanLyCuaHangTheThaoContext _db;

    public TrangChuController(QuanLyCuaHangTheThaoContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }
}
