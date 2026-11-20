using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblMauSac
{
    public int IMaMau { get; set; }

    public string STenMau { get; set; } = null!;

    public virtual ICollection<TblSanPhamTblMauSac> TblSanPhamTblMauSacs { get; set; } = new List<TblSanPhamTblMauSac>();
}
