using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblChiTietHoaDon
{
    public int IMaHd { get; set; }

    public int IMaSp { get; set; }

    public int IMaSize { get; set; }

    public int IMaMau { get; set; }

    public int ISoLuong { get; set; }

    public decimal FDonGia { get; set; }

    public virtual TblHoaDon IMaHdNavigation { get; set; } = null!;

    public virtual TblSanPhamTblKichThuoc TblSanPhamTblKichThuoc { get; set; } = null!;

    public virtual TblSanPhamTblMauSac TblSanPhamTblMauSac { get; set; } = null!;
}
