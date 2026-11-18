using System.Globalization;

namespace QuanLyCuaHangTheThao.Services;

/// <summary>
/// Tinh toan lich lam viec theo tuan (T2 -> CN). Bien tach rieng de unit test
/// khong can database, giong PasswordHasher la static class.
/// </summary>
public static class LichLamViecTinh
{
    /// <summary>So ngay trong mot tuan (T2 -> CN).</summary>
    public const int SoNgayTuan = 7;

    private static readonly string[] TenThu =
    {
        "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật"
    };

    /// <summary>Ngay dau tuan (Thu 2) chua phan gioi thoi gian cua ngay truyen vao.</summary>
    public static DateTime DauTuan(DateTime ngay)
    {
        int lech = ((int)ngay.DayOfWeek + 6) % 7;
        return ngay.Date.AddDays(-lech);
    }

    /// <summary>Ngay cuoi tuan (Chu nhat) cua ngay truyen vao.</summary>
    public static DateTime CuoiTuan(DateTime ngay) => DauTuan(ngay).AddDays(SoNgayTuan - 1);

    /// <summary>7 ngay trong tuan, bat dau tu Thu 2.</summary>
    public static DateTime[] NgayTrongTuan(DateTime ngay)
    {
        var dauTuan = DauTuan(ngay);
        var ketQua = new DateTime[SoNgayTuan];
        for (int i = 0; i < SoNgayTuan; i++)
            ketQua[i] = dauTuan.AddDays(i);
        return ketQua;
    }

    /// <summary>Ten thu theo chi so 0..6 (0 = Thu 2).</summary>
    public static string TenThuTrongTuan(int chiSo) =>
        chiSo >= 0 && chiSo < TenThu.Length ? TenThu[chiSo] : "";

    /// <summary>Ca vượt qua nua dem (vi du 23:00 -> 07:00).</summary>
    public static bool LaCaQuaDem(TimeSpan gioBatDau, TimeSpan gioKetThuc) => gioKetThuc <= gioBatDau;

    /// <summary>
    /// Do dai ca. Ca qua nua dem se cong them sang ngay hom sau.
    /// Tra TimeSpan.Zero khi gio bat dau bang gio ket thuc.
    /// </summary>
    public static TimeSpan ThoiLuong(TimeSpan gioBatDau, TimeSpan gioKetThuc)
    {
        if (gioBatDau == gioKetThuc) return TimeSpan.Zero;
        if (gioKetThuc > gioBatDau) return gioKetThuc - gioBatDau;
        return TimeSpan.FromTicks(TimeSpan.TicksPerDay - gioBatDau.Ticks) + gioKetThuc;
    }

    /// <summary>Thoi luong ca tinh ra so gio (lam tron 2 chu so thap phan).</summary>
    public static decimal SoGioCa(TimeSpan gioBatDau, TimeSpan gioKetThuc) =>
        Math.Round((decimal)ThoiLuong(gioBatDau, gioKetThuc).TotalHours, 2);

    /// <summary>Dinh dang gio HH:mm de gan vao input type="time" (khong phu thuoc culture).</summary>
    public static string FormatGio(TimeSpan gio) =>
        gio.ToString(@"hh\:mm", CultureInfo.InvariantCulture);

    /// <summary>Dinh dang khoang ngay cua tuan: dd/MM/yyyy - dd/MM/yyyy.</summary>
    public static string FormatKhoangTuan(DateTime ngay)
    {
        var dau = DauTuan(ngay);
        var cuoi = dau.AddDays(SoNgayTuan - 1);
        return $"{dau:dd/MM/yyyy} - {cuoi:dd/MM/yyyy}";
    }

    /// <summary>
    /// Ca hop le khi gio bat dau khac gio ket thuc.
    /// Khop voi CHECK constraint CK_tblCaLamViec_Gio tren database.
    /// </summary>
    public static bool LaCaHopLe(TimeSpan gioBatDau, TimeSpan gioKetThuc) => gioBatDau != gioKetThuc;
}