namespace QuanLyCuaHangTheThao.Models;

public class PhieuNhapViewModel
{
    public int IMaNcc { get; set; }
    public DateTime DNgayNhap { get; set; } = DateTime.Now;
    public List<PhieuNhapLine> Lines { get; set; } = new();
}

public class PhieuNhapLine
{
    public int IMaSp { get; set; }
    public int IMaSize { get; set; }
    public int IMaMau { get; set; }
    public int ISoLuong { get; set; }
    public decimal FDonGiaNhap { get; set; }
}
