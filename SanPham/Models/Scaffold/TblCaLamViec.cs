using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblCaLamViec
{
    public int IMaCa { get; set; }

    public string STenCa { get; set; } = null!;

    public TimeSpan DGioBatDau { get; set; }

    public TimeSpan DGioKetThuc { get; set; }

    public virtual ICollection<TblLichLamViec> TblLichLamViecs { get; set; } = new List<TblLichLamViec>();
}