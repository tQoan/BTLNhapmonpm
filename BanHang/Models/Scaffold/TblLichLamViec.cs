using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblLichLamViec
{
    public int IMaLich { get; set; }

    public int IMaNv { get; set; }

    public int IMaCa { get; set; }

    public DateTime DNgayLam { get; set; }

    public string? SGhiChu { get; set; }

    public virtual TblCaLamViec IMaCaNavigation { get; set; } = null!;

    public virtual TblNhanVien IMaNvNavigation { get; set; } = null!;
}