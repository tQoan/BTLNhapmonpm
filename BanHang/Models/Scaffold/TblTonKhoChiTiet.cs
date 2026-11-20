using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblTonKhoChiTiet
{
    public int IMaSp { get; set; }

    public int IMaSize { get; set; }

    public int IMaMau { get; set; }

    public int ISoLuongTon { get; set; }

    public virtual TblSanPhamTblKichThuoc TblSanPhamTblKichThuoc { get; set; } = null!;

    public virtual TblSanPhamTblMauSac TblSanPhamTblMauSac { get; set; } = null!;
}
