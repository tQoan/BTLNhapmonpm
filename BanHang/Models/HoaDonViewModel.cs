namespace QuanLyCuaHangTheThao.Models;

public class HoaDonViewModel
{
    public int? IMaKh { get; set; }
    public decimal FGiamGia { get; set; }
    public string SPhuongThucTt { get; set; } = "Tiền mặt";
    public decimal FTienKhachTra { get; set; }
    public List<HoaDonLine> Lines { get; set; } = new();
}

public class HoaDonLine
{
    public int IMaSp { get; set; }
    public int IMaSize { get; set; }
    public int IMaMau { get; set; }
    public int ISoLuong { get; set; }
    public decimal FDonGia { get; set; }
    public int TonKho { get; set; }
}
