using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblKhachHang
{
    public int IMaKh { get; set; }

    public string STenKh { get; set; } = null!;

    public string? SDienThoai { get; set; }

    public string? SDiaChi { get; set; }

    public virtual ICollection<TblHoaDon> TblHoaDons { get; set; } = new List<TblHoaDon>();
}
