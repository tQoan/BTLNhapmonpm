using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblChiTietPhieuNhap
{
    public int IMaPn { get; set; }

    public int IMaSp { get; set; }

    public int IMaSize { get; set; }

    public int IMaMau { get; set; }

    public int ISoLuong { get; set; }

    public decimal FDonGiaNhap { get; set; }

    public virtual TblPhieuNhap IMaPnNavigation { get; set; } = null!;

    public virtual TblSanPhamTblKichThuoc TblSanPhamTblKichThuoc { get; set; } = null!;

    public virtual TblSanPhamTblMauSac TblSanPhamTblMauSac { get; set; } = null!;
}
