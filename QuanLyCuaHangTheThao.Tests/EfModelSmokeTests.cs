using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;
using Xunit;

namespace QuanLyCuaHangTheThao.Tests;

public class EfModelSmokeTests
{
    private static QuanLyCuaHangTheThaoContext TaoContext()
    {
        var options = new DbContextOptionsBuilder<QuanLyCuaHangTheThaoContext>()
            .UseSqlServer("Server=localhost;Database=KhongKetNoi;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;
        return new QuanLyCuaHangTheThaoContext(options);
    }

    [Fact]
    public void Model_ShouldBuild_WithoutValidationErrors()
    {
        using var db = TaoContext();
        // Force EF to validate the whole model (key, FK, index, column type).
        Assert.Empty(db.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties()
                .Where(p => p.GetColumnType() != null && p.GetColumnName() == null)
                .Select(p => $"{e.ClrType.Name}.{p.Name}"))
            .ToList());
    }

    [Fact]
    public void CaLamViec_ShouldBeMappedToExpectedTableAndColumns()
    {
        using var db = TaoContext();
        var e = db.Model.FindEntityType(typeof(TblCaLamViec))!;

        Assert.Equal("tblCaLamViec", e.GetTableName());
        Assert.Equal("iMaCa", e.FindProperty(nameof(TblCaLamViec.IMaCa))!.GetColumnName());
        Assert.Equal("sTenCa", e.FindProperty(nameof(TblCaLamViec.STenCa))!.GetColumnName());
        Assert.Equal("dGioBatDau", e.FindProperty(nameof(TblCaLamViec.DGioBatDau))!.GetColumnName());
        Assert.Equal("dGioKetThuc", e.FindProperty(nameof(TblCaLamViec.DGioKetThuc))!.GetColumnName());
        Assert.Equal("time", e.FindProperty(nameof(TblCaLamViec.DGioBatDau))!.GetColumnType());
    }

    [Fact]
    public void LichLamViec_ShouldBeMappedWithUniqueEmployeePerDayIndex()
    {
        using var db = TaoContext();
        var e = db.Model.FindEntityType(typeof(TblLichLamViec))!;

        Assert.Equal("tblLichLamViec", e.GetTableName());
        Assert.Equal("date", e.FindProperty(nameof(TblLichLamViec.DNgayLam))!.GetColumnType());

        var index = e.GetIndexes()
            .FirstOrDefault(i => i.GetDatabaseName() == "UQ_tblLichLamViec_NV_Ngay");
        Assert.NotNull(index);
        Assert.True(index!.IsUnique);
        Assert.Equal(
            new[] { nameof(TblLichLamViec.IMaNv), nameof(TblLichLamViec.DNgayLam) },
            index.Properties.Select(p => p.Name).ToArray());
    }

    [Fact]
    public void LichLamViec_ShouldHaveBothForeignKeys()
    {
        using var db = TaoContext();
        var e = db.Model.FindEntityType(typeof(TblLichLamViec))!;

        var fks = e.GetForeignKeys()
            .Select(f => f.GetConstraintName())
            .OrderBy(n => n)
            .ToArray();

        Assert.Equal(
            new[] { "FK_tblLichLamViec_tblCaLamViec", "FK_tblLichLamViec_tblNhanVien" },
            fks);
    }

    [Fact]
    public void NhanVien_ShouldExposeLichLamViecNavigation()
    {
        using var db = TaoContext();
        var e = db.Model.FindEntityType(typeof(TblNhanVien))!;
        var nav = e.FindNavigation(nameof(TblNhanVien.TblLichLamViecs));

        Assert.NotNull(nav);
        Assert.True(nav!.IsCollection);
        Assert.Equal(typeof(TblLichLamViec), nav.TargetEntityType.ClrType);
    }
}