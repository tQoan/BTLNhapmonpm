using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblPhieuNhap
{
    public int IMaPn { get; set; }

    public DateTime DNgayNhap { get; set; }

    public int IMaNcc { get; set; }

    public int IMaNv { get; set; }

    public virtual TblNhaCungCap IMaNccNavigation { get; set; } = null!;

    public virtual TblNhanVien IMaNvNavigation { get; set; } = null!;

    public virtual ICollection<TblChiTietPhieuNhap> TblChiTietPhieuNhaps { get; set; } = new List<TblChiTietPhieuNhap>();
}
