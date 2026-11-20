using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class VBaoCaoDoanhThu
{
    public DateOnly? DNgay { get; set; }

    public int IMaHd { get; set; }

    public decimal? FDoanhThuTruocGiam { get; set; }

    public decimal? FDoanhThuThucThu { get; set; }

    public decimal? FGiaVon { get; set; }

    public decimal? FLoiNhuanGop { get; set; }
}
