using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class VThi
{
    public int IMaSp { get; set; }

    public string STenSp { get; set; } = null!;

    public string? SXuatXu { get; set; }

    public int IMaSize { get; set; }

    public string STenSize { get; set; } = null!;

    public int IMaMau { get; set; }

    public string STenMau { get; set; } = null!;

    public int ISoLuongTon { get; set; }

    public DateOnly? DNgayHetHan { get; set; }

    public string STrangThaiHanDung { get; set; } = null!;
}
