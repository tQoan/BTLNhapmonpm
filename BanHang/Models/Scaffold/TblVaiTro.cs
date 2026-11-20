using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblVaiTro
{
    public int IMaVaiTro { get; set; }

    public string STenVaiTro { get; set; } = null!;

    public virtual ICollection<TblNhanVien> TblNhanViens { get; set; } = new List<TblNhanVien>();
}
