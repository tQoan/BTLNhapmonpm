using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class TblHoaDon
{
    public int IMaHd { get; set; }

    public DateTime DNgayLap { get; set; }

    public int? IMaKh { get; set; }

    public int IMaNv { get; set; }

    public decimal FGiamGia { get; set; }

    public decimal FTongTien { get; set; }

    public decimal FThanhToan { get; set; }

    public string SPhuongThucTt { get; set; } = null!;

    public decimal FTienKhachTra { get; set; }

    public decimal FTienTraLai { get; set; }

    public virtual TblKhachHang? IMaKhNavigation { get; set; }

    public virtual TblNhanVien IMaNvNavigation { get; set; } = null!;

    public virtual ICollection<TblChiTietHoaDon> TblChiTietHoaDons { get; set; } = new List<TblChiTietHoaDon>();
}
