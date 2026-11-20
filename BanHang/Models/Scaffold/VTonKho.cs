using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class VTonKho
{
    public int IMaSp { get; set; }

    public string STenSp { get; set; } = null!;

    public int IMaSize { get; set; }

    public string STenSize { get; set; } = null!;

    public int IMaMau { get; set; }

    public string STenMau { get; set; } = null!;

    public int FTongNhap { get; set; }

    public int FTongBan { get; set; }

    public int? ISoLuongTon { get; set; }
}
