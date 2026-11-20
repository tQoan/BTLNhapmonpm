using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblQuanAo
{
    public int IMaSp { get; set; }

    public string? SChatLieu { get; set; }

    public string? SKieuDang { get; set; }

    public virtual TblSanPham IMaSpNavigation { get; set; } = null!;
}
