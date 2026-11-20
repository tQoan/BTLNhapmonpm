using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblKichThuoc
{
    public int IMaSize { get; set; }

    public string STenSize { get; set; } = null!;

    public virtual ICollection<TblSanPhamTblKichThuoc> TblSanPhamTblKichThuocs { get; set; } = new List<TblSanPhamTblKichThuoc>();
}
