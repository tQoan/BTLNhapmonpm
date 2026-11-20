using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblTaiKhoan
{
    public string STenDangNhap { get; set; } = null!;

    public string SMatKhau { get; set; } = null!;

    public int IMaNv { get; set; }

    public virtual TblNhanVien IMaNvNavigation { get; set; } = null!;
}
