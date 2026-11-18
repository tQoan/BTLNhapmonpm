using QuanLyCuaHangTheThao.Models.Scaffold;

namespace QuanLyCuaHangTheThao.Models;

/// <summary>
/// Luoi lich lam viec theo tuan: moi nhan vien la mot hang, moi ngay la mot o.
/// Moi o giu IMaLich de biet lich da ton tai hay chua, va IMaCa de gan ca.
/// </summary>
public class LichLamViecViewModel
{
    /// <summary>Ngay dau tuan (luon la Thu 2).</summary>
    public DateTime DauTuan { get; set; }

    public List<TblCaLamViec> DanhSachCa { get; set; } = new();
    public List<HangLichLamViec> Luoi { get; set; } = new();
    public string? GhiChu { get; set; }
}

public class HangLichLamViec
{
    public int IMaNv { get; set; }
    public string STenNv { get; set; } = null!;
    public string? SDienThoai { get; set; }
    public string? SVaiTro { get; set; }
    public LichLamViecO[] Os { get; set; } = new LichLamViecO[7];
}

public class LichLamViecO
{
    public int? IMaLich { get; set; }
    public int? IMaCa { get; set; }
}