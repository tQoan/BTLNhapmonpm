using System;
using System.Collections.Generic;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class VInHoaDon
{
    public int IMaHd { get; set; }

    public DateTime DNgayLap { get; set; }

    public int? IMaKh { get; set; }

    public string? STenKh { get; set; }

    public string? SDienThoaiKh { get; set; }

    public int IMaNv { get; set; }

    public string STenNv { get; set; } = null!;

    public int IMaSp { get; set; }

    public string STenSp { get; set; } = null!;

    public string STenSize { get; set; } = null!;

    public string STenMau { get; set; } = null!;

    public int ISoLuong { get; set; }

    public decimal FDonGia { get; set; }

    public decimal? FThanhTienGoc { get; set; }

    public decimal? FThanhTienSauGiam10 { get; set; }

    public decimal? FTienGiam10 { get; set; }

    public decimal FGiamGia { get; set; }

    public decimal FTongTien { get; set; }

    public decimal FThanhToan { get; set; }

    public string SPhuongThucTt { get; set; } = null!;

    public decimal FTienKhachTra { get; set; }

    public decimal FTienTraLai { get; set; }
}
