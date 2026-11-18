using QuanLyCuaHangTheThao.Services;
using Xunit;

namespace QuanLyCuaHangTheThao.Tests;

public class LichLamViecTinhTests
{
    // Tuan chuan T2 -> CN: 01/06/2026 (T2) - 07/06/2026 (CN)
    private static readonly DateTime ThuHai = new(2026, 6, 1);
    private static readonly DateTime ChuNhat = new(2026, 6, 7);
    private static readonly DateTime ThuHaiSau = new(2026, 6, 8);

    [Fact]
    public void DauTuan_ShouldReturnMonday()
    {
        Assert.Equal(DayOfWeek.Monday, LichLamViecTinh.DauTuan(ChuNhat).DayOfWeek);
        Assert.Equal(DayOfWeek.Monday, LichLamViecTinh.DauTuan(ThuHai).DayOfWeek);
    }

    [Fact]
    public void DauTuan_SundayEndsItsOwnWeek()
    {
        // Chu nhat 07/06 la ngay cuoi cua tuan 01/06 -> 07/06,
        // con thu hai 08/06 da sang tuan moi.
        Assert.Equal(ThuHai, LichLamViecTinh.DauTuan(ChuNhat));
        Assert.Equal(ThuHaiSau, LichLamViecTinh.DauTuan(ThuHaiSau));
        Assert.NotEqual(LichLamViecTinh.DauTuan(ChuNhat), LichLamViecTinh.DauTuan(ThuHaiSau));
    }

    [Fact]
    public void DauTuan_MondayIsItself()
    {
        Assert.Equal(ThuHai, LichLamViecTinh.DauTuan(ThuHai));
    }

    [Fact]
    public void DauTuan_ShouldStripTime()
    {
        var coGio = ThuHai.AddHours(23).AddMinutes(45);
        Assert.Equal(ThuHuan(ThuHai), LichLamViecTinh.DauTuan(coGio));
    }

    [Fact]
    public void CuoiTuan_ShouldReturnSunday()
    {
        Assert.Equal(DayOfWeek.Sunday, LichLamViecTinh.CuoiTuan(ThuHai).DayOfWeek);
        Assert.Equal(ThuHai.AddDays(6), LichLamViecTinh.CuoiTuan(ThuHai));
    }

    [Fact]
    public void NgayTrongTuan_ShouldReturnSevenDaysStartingMonday()
    {
        var ngay = LichLamViecTinh.NgayTrongTuan(ChuNhat);

        Assert.Equal(LichLamViecTinh.SoNgayTuan, ngay.Length);
        Assert.Equal(ThuHai, ngay[0]);
        Assert.Equal(ChuNhat, ngay[6]);
        Assert.Equal(DayOfWeek.Monday, ngay[0].DayOfWeek);
        Assert.Equal(DayOfWeek.Sunday, ngay[6].DayOfWeek);
        for (int i = 1; i < ngay.Length; i++)
            Assert.Equal(ngay[i - 1].AddDays(1), ngay[i]);
    }

    [Fact]
    public void NgayTrongTuan_ShouldCoverSevenDistinctDates()
    {
        var ngay = LichLamViecTinh.NgayTrongTuan(DateTime.Today);
        Assert.Equal(7, ngay.Distinct().Count());
    }

    [Fact]
    public void TenThuTrongTuan_ShouldIndexMondayFirst()
    {
        Assert.Equal("Thứ 2", LichLamViecTinh.TenThuTrongTuan(0));
        Assert.Equal("Chủ nhật", LichLamViecTinh.TenThuTrongTuan(6));
    }

    [Fact]
    public void TenThuTrongTuan_ShouldReturnEmpty_ForOutOfRange()
    {
        Assert.Equal("", LichLamViecTinh.TenThuTrongTuan(-1));
        Assert.Equal("", LichLamViecTinh.TenThuTrongTuan(7));
    }

    [Theory]
    [InlineData("07:00", "15:00", false)]
    [InlineData("15:00", "23:00", false)]
    [InlineData("23:00", "07:00", true)]
    [InlineData("22:00", "06:00", true)]
    public void LaCaQuaDem(string batDau, string ketThuc, bool quaDem)
    {
        Assert.Equal(quaDem, LichLamViecTinh.LaCaQuaDem(TimeSpan.Parse(batDau), TimeSpan.Parse(ketThuc)));
    }

    [Theory]
    [InlineData("07:00", "15:00", 8)]
    [InlineData("15:00", "23:00", 8)]
    [InlineData("23:00", "07:00", 8)]
    [InlineData("22:00", "06:00", 8)]
    [InlineData("08:00", "12:30", 4.5)]
    public void SoGioCa_ShouldHandleOvernightShifts(string batDau, string ketThuc, double expected)
    {
        Assert.Equal((decimal)expected, LichLamViecTinh.SoGioCa(TimeSpan.Parse(batDau), TimeSpan.Parse(ketThuc)));
    }

    [Fact]
    public void ThoiLuong_ShouldBeZero_WhenTimesAreEqual()
    {
        var gio = new TimeSpan(8, 0, 0);
        Assert.Equal(TimeSpan.Zero, LichLamViecTinh.ThoiLuong(gio, gio));
    }

    [Fact]
    public void ThoiLuong_ShouldWrapPastMidnight()
    {
        var thoiLuong = LichLamViecTinh.ThoiLuong(new TimeSpan(23, 0, 0), new TimeSpan(7, 0, 0));
        Assert.Equal(new TimeSpan(8, 0, 0), thoiLuong);
    }

    [Fact]
    public void FormatGio_ShouldAlwaysBeHhMm()
    {
        Assert.Equal("07:00", LichLamViecTinh.FormatGio(new TimeSpan(7, 0, 0)));
        Assert.Equal("23:00", LichLamViecTinh.FormatGio(new TimeSpan(23, 0, 0)));
        Assert.Equal("07:05", LichLamViecTinh.FormatGio(new TimeSpan(7, 5, 42)));
    }

    [Fact]
    public void FormatKhoangTuan_ShouldSpanMondayToSunday()
    {
        Assert.Equal("01/06/2026 - 07/06/2026", LichLamViecTinh.FormatKhoangTuan(ChuNhat));
        Assert.Equal("08/06/2026 - 14/06/2026", LichLamViecTinh.FormatKhoangTuan(ThuHaiSau));
    }

    [Fact]
    public void LaCaHopLe_ShouldRejectIdenticalTimes()
    {
        var gio = new TimeSpan(8, 0, 0);
        Assert.False(LichLamViecTinh.LaCaHopLe(gio, gio));
        Assert.True(LichLamViecTinh.LaCaHopLe(gio, new TimeSpan(16, 0, 0)));
        Assert.True(LichLamViecTinh.LaCaHopLe(new TimeSpan(23, 0, 0), new TimeSpan(7, 0, 0)));
    }

    private static DateTime ThuHuan(DateTime ngay) => new(ngay.Year, ngay.Month, ngay.Day);
}