using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblSanPhamTblMauSac
{
    public int IMaSp { get; set; }

    public int IMaMau { get; set; }

    public virtual TblMauSac IMaMauNavigation { get; set; } = null!;

    public virtual TblSanPham IMaSpNavigation { get; set; } = null!;

    public virtual ICollection<TblChiTietHoaDon> TblChiTietHoaDons { get; set; } = new List<TblChiTietHoaDon>();

    public virtual ICollection<TblChiTietPhieuNhap> TblChiTietPhieuNhaps { get; set; } = new List<TblChiTietPhieuNhap>();

    public virtual ICollection<TblTonKhoChiTiet> TblTonKhoChiTiets { get; set; } = new List<TblTonKhoChiTiet>();
}
