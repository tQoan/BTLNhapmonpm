using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblNhanVien
{
    public int IMaNv { get; set; }

    public string STenNv { get; set; } = null!;

    public string? SGioiTinh { get; set; }

    public string? SDienThoai { get; set; }

    public int IMaVaiTro { get; set; }

    public virtual TblVaiTro IMaVaiTroNavigation { get; set; } = null!;

    public virtual ICollection<TblLichLamViec> TblLichLamViecs { get; set; } = new List<TblLichLamViec>();

    public virtual ICollection<TblHoaDon> TblHoaDons { get; set; } = new List<TblHoaDon>();

    public virtual ICollection<TblPhieuNhap> TblPhieuNhaps { get; set; } = new List<TblPhieuNhap>();

    public virtual TblTaiKhoan? TblTaiKhoan { get; set; }
}
