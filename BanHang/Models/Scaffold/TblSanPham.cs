using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblSanPham
{
    public int IMaSp { get; set; }

    public string STenSp { get; set; } = null!;

    public int IMaNcc { get; set; }

    public string? SXuatXu { get; set; }

    public decimal FGiaNhap { get; set; }

    public decimal FGiaBan { get; set; }

    public string? SMoTa { get; set; }

    public int ISoLuongTon { get; set; }

    public DateOnly? DNgayHetHan { get; set; }

    public virtual TblNhaCungCap IMaNccNavigation { get; set; } = null!;

    public virtual TblDungCu? TblDungCu { get; set; }

    public virtual TblGiayDep? TblGiayDep { get; set; }

    public virtual TblQuanAo? TblQuanAo { get; set; }

    public virtual ICollection<TblSanPhamTblKichThuoc> TblSanPhamTblKichThuocs { get; set; } = new List<TblSanPhamTblKichThuoc>();

    public virtual ICollection<TblSanPhamTblMauSac> TblSanPhamTblMauSacs { get; set; } = new List<TblSanPhamTblMauSac>();
}
