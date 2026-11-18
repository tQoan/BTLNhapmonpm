using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblNhaCungCap
{
    public int IMaNcc { get; set; }

    public string STenNcc { get; set; } = null!;

    public string? SDiaChi { get; set; }

    public string? SDienThoai { get; set; }

    public virtual ICollection<TblPhieuNhap> TblPhieuNhaps { get; set; } = new List<TblPhieuNhap>();

    public virtual ICollection<TblSanPham> TblSanPhams { get; set; } = new List<TblSanPham>();
}
