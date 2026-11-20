using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCuaHangTheThao.Models.Scaffold;

public partial class QuanLyCuaHangTheThaoContext : DbContext
{
    public QuanLyCuaHangTheThaoContext()
    {
    }

    public QuanLyCuaHangTheThaoContext(DbContextOptions<QuanLyCuaHangTheThaoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblCaLamViec> TblCaLamViecs { get; set; }

    public virtual DbSet<TblChiTietHoaDon> TblChiTietHoaDons { get; set; }

    public virtual DbSet<TblChiTietPhieuNhap> TblChiTietPhieuNhaps { get; set; }

    public virtual DbSet<TblDungCu> TblDungCus { get; set; }

    public virtual DbSet<TblGiayDep> TblGiayDeps { get; set; }

    public virtual DbSet<TblHoaDon> TblHoaDons { get; set; }

    public virtual DbSet<TblKhachHang> TblKhachHangs { get; set; }

    public virtual DbSet<TblKichThuoc> TblKichThuocs { get; set; }

    public virtual DbSet<TblLichLamViec> TblLichLamViecs { get; set; }

    public virtual DbSet<TblMauSac> TblMauSacs { get; set; }

    public virtual DbSet<TblNhaCungCap> TblNhaCungCaps { get; set; }

    public virtual DbSet<TblNhanVien> TblNhanViens { get; set; }

    public virtual DbSet<TblPhieuNhap> TblPhieuNhaps { get; set; }

    public virtual DbSet<TblQuanAo> TblQuanAos { get; set; }

    public virtual DbSet<TblSanPham> TblSanPhams { get; set; }

    public virtual DbSet<TblSanPhamTblKichThuoc> TblSanPhamTblKichThuocs { get; set; }

    public virtual DbSet<TblSanPhamTblMauSac> TblSanPhamTblMauSacs { get; set; }

    public virtual DbSet<TblTaiKhoan> TblTaiKhoans { get; set; }

    public virtual DbSet<TblTonKhoChiTiet> TblTonKhoChiTiets { get; set; }

    public virtual DbSet<TblVaiTro> TblVaiTros { get; set; }

    public virtual DbSet<VBaoCaoDoanhThu> VBaoCaoDoanhThus { get; set; }

    public virtual DbSet<VInHoaDon> VInHoaDons { get; set; }

    public virtual DbSet<VThi> VThis { get; set; }

    public virtual DbSet<VTonKho> VTonKhos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblCaLamViec>(entity =>
        {
            entity.HasKey(e => e.IMaCa);

            entity.ToTable("tblCaLamViec");

            entity.HasIndex(e => e.STenCa, "UQ_tblCaLamViec_sTenCa").IsUnique();

            entity.Property(e => e.IMaCa)
                .ValueGeneratedOnAdd()
                .HasColumnName("iMaCa");
            entity.Property(e => e.STenCa)
                .HasMaxLength(100)
                .IsRequired()
                .HasColumnName("sTenCa");
            entity.Property(e => e.DGioBatDau)
                .HasColumnType("time")
                .HasColumnName("dGioBatDau");
            entity.Property(e => e.DGioKetThuc)
                .HasColumnType("time")
                .HasColumnName("dGioKetThuc");
        });

        modelBuilder.Entity<TblChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => new { e.IMaHd, e.IMaSp, e.IMaSize, e.IMaMau });

            entity.ToTable("tblChiTietHoaDon", tb => tb.HasTrigger("trg_ChiTietHoaDon_KiemTraTonVaCapNhat"));

            entity.Property(e => e.IMaHd).HasColumnName("iMaHD");
            entity.Property(e => e.IMaSp).HasColumnName("iMaSP");
            entity.Property(e => e.IMaSize).HasColumnName("iMaSize");
            entity.Property(e => e.IMaMau).HasColumnName("iMaMau");
            entity.Property(e => e.FDonGia)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fDonGia");
            entity.Property(e => e.ISoLuong).HasColumnName("iSoLuong");

            entity.HasOne(d => d.IMaHdNavigation).WithMany(p => p.TblChiTietHoaDons)
                .HasForeignKey(d => d.IMaHd)
                .HasConstraintName("FK_CTHD_HoaDon");

            entity.HasOne(d => d.TblSanPhamTblMauSac).WithMany(p => p.TblChiTietHoaDons)
                .HasForeignKey(d => new { d.IMaSp, d.IMaMau })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTHD_SP_Mau");

            entity.HasOne(d => d.TblSanPhamTblKichThuoc).WithMany(p => p.TblChiTietHoaDons)
                .HasForeignKey(d => new { d.IMaSp, d.IMaSize })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTHD_SP_Size");
        });

        modelBuilder.Entity<TblChiTietPhieuNhap>(entity =>
        {
            entity.HasKey(e => new { e.IMaPn, e.IMaSp, e.IMaSize, e.IMaMau });

            entity.ToTable("tblChiTietPhieuNhap", tb => tb.HasTrigger("trg_ChiTietPhieuNhap_CapNhatTonKho"));

            entity.Property(e => e.IMaPn).HasColumnName("iMaPN");
            entity.Property(e => e.IMaSp).HasColumnName("iMaSP");
            entity.Property(e => e.IMaSize).HasColumnName("iMaSize");
            entity.Property(e => e.IMaMau).HasColumnName("iMaMau");
            entity.Property(e => e.FDonGiaNhap)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fDonGiaNhap");
            entity.Property(e => e.ISoLuong).HasColumnName("iSoLuong");

            entity.HasOne(d => d.IMaPnNavigation).WithMany(p => p.TblChiTietPhieuNhaps)
                .HasForeignKey(d => d.IMaPn)
                .HasConstraintName("FK_CTPN_PhieuNhap");

            entity.HasOne(d => d.TblSanPhamTblMauSac).WithMany(p => p.TblChiTietPhieuNhaps)
                .HasForeignKey(d => new { d.IMaSp, d.IMaMau })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPN_SP_Mau");

            entity.HasOne(d => d.TblSanPhamTblKichThuoc).WithMany(p => p.TblChiTietPhieuNhaps)
                .HasForeignKey(d => new { d.IMaSp, d.IMaSize })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTPN_SP_Size");
        });

        modelBuilder.Entity<TblDungCu>(entity =>
        {
            entity.HasKey(e => e.IMaSp);

            entity.ToTable("tblDungCu");

            entity.Property(e => e.IMaSp)
                .ValueGeneratedNever()
                .HasColumnName("iMaSP");
            entity.Property(e => e.SChatLieu)
                .HasMaxLength(100)
                .HasColumnName("sChatLieu");
            entity.Property(e => e.SCongDung)
                .HasMaxLength(200)
                .HasColumnName("sCongDung");

            entity.HasOne(d => d.IMaSpNavigation).WithOne(p => p.TblDungCu)
                .HasForeignKey<TblDungCu>(d => d.IMaSp)
                .HasConstraintName("FK_tblDungCu_tblSanPham");
        });

        modelBuilder.Entity<TblGiayDep>(entity =>
        {
            entity.HasKey(e => e.IMaSp);

            entity.ToTable("tblGiayDep");

            entity.Property(e => e.IMaSp)
                .ValueGeneratedNever()
                .HasColumnName("iMaSP");
            entity.Property(e => e.SChatLieu)
                .HasMaxLength(100)
                .HasColumnName("sChatLieu");
            entity.Property(e => e.SLoaiDe)
                .HasMaxLength(100)
                .HasColumnName("sLoaiDe");

            entity.HasOne(d => d.IMaSpNavigation).WithOne(p => p.TblGiayDep)
                .HasForeignKey<TblGiayDep>(d => d.IMaSp)
                .HasConstraintName("FK_tblGiayDep_tblSanPham");
        });

        modelBuilder.Entity<TblHoaDon>(entity =>
        {
            entity.HasKey(e => e.IMaHd);

            entity.ToTable("tblHoaDon");

            entity.Property(e => e.IMaHd).HasColumnName("iMaHD");
            entity.Property(e => e.DNgayLap)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("dNgayLap");
            entity.Property(e => e.FGiamGia)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fGiamGia");
            entity.Property(e => e.FThanhToan)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fThanhToan");
            entity.Property(e => e.FTienKhachTra)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fTienKhachTra");
            entity.Property(e => e.FTienTraLai)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fTienTraLai");
            entity.Property(e => e.FTongTien)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fTongTien");
            entity.Property(e => e.IMaKh).HasColumnName("iMaKH");
            entity.Property(e => e.IMaNv).HasColumnName("iMaNV");
            entity.Property(e => e.SPhuongThucTt)
                .HasMaxLength(50)
                .HasDefaultValue("Tiền mặt")
                .HasColumnName("sPhuongThucTT");

            entity.HasOne(d => d.IMaKhNavigation).WithMany(p => p.TblHoaDons)
                .HasForeignKey(d => d.IMaKh)
                .HasConstraintName("FK_tblHoaDon_tblKhachHang");

            entity.HasOne(d => d.IMaNvNavigation).WithMany(p => p.TblHoaDons)
                .HasForeignKey(d => d.IMaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblHoaDon_tblNhanVien");
        });

        modelBuilder.Entity<TblKhachHang>(entity =>
        {
            entity.HasKey(e => e.IMaKh);

            entity.ToTable("tblKhachHang");

            entity.HasIndex(e => e.SDienThoai, "UQ_tblKhachHang_sDienThoai").IsUnique();

            entity.Property(e => e.IMaKh).HasColumnName("iMaKH");
            entity.Property(e => e.SDiaChi)
                .HasMaxLength(255)
                .HasColumnName("sDiaChi");
            entity.Property(e => e.SDienThoai)
                .HasMaxLength(20)
                .HasColumnName("sDienThoai");
            entity.Property(e => e.STenKh)
                .HasMaxLength(150)
                .HasColumnName("sTenKH");
        });

        modelBuilder.Entity<TblKichThuoc>(entity =>
        {
            entity.HasKey(e => e.IMaSize);

            entity.ToTable("tblKichThuoc");

            entity.HasIndex(e => e.STenSize, "UQ_tblKichThuoc_sTenSize").IsUnique();

            entity.Property(e => e.IMaSize).HasColumnName("iMaSize");
            entity.Property(e => e.STenSize)
                .HasMaxLength(20)
                .HasColumnName("sTenSize");
        });

        modelBuilder.Entity<TblLichLamViec>(entity =>
        {
            entity.HasKey(e => e.IMaLich);

            entity.ToTable("tblLichLamViec");

            entity.HasIndex(e => new { e.IMaNv, e.DNgayLam }, "UQ_tblLichLamViec_NV_Ngay").IsUnique();

            entity.Property(e => e.IMaLich)
                .ValueGeneratedOnAdd()
                .HasColumnName("iMaLich");
            entity.Property(e => e.IMaNv)
                .HasColumnName("iMaNV");
            entity.Property(e => e.IMaCa)
                .HasColumnName("iMaCa");
            entity.Property(e => e.DNgayLam)
                .HasColumnType("date")
                .HasColumnName("dNgayLam");
            entity.Property(e => e.SGhiChu)
                .HasMaxLength(255)
                .HasColumnName("sGhiChu");

            entity.HasOne(d => d.IMaNvNavigation).WithMany(p => p.TblLichLamViecs)
                .HasForeignKey(d => d.IMaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblLichLamViec_tblNhanVien");

            entity.HasOne(d => d.IMaCaNavigation).WithMany(p => p.TblLichLamViecs)
                .HasForeignKey(d => d.IMaCa)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblLichLamViec_tblCaLamViec");
        });

        modelBuilder.Entity<TblMauSac>(entity =>
        {
            entity.HasKey(e => e.IMaMau);

            entity.ToTable("tblMauSac");

            entity.HasIndex(e => e.STenMau, "UQ_tblMauSac_sTenMau").IsUnique();

            entity.Property(e => e.IMaMau).HasColumnName("iMaMau");
            entity.Property(e => e.STenMau)
                .HasMaxLength(50)
                .HasColumnName("sTenMau");
        });

        modelBuilder.Entity<TblNhaCungCap>(entity =>
        {
            entity.HasKey(e => e.IMaNcc);

            entity.ToTable("tblNhaCungCap");

            entity.HasIndex(e => e.SDienThoai, "UQ_tblNhaCungCap_sDienThoai").IsUnique();

            entity.HasIndex(e => e.STenNcc, "UQ_tblNhaCungCap_sTenNCC").IsUnique();

            entity.Property(e => e.IMaNcc).HasColumnName("iMaNCC");
            entity.Property(e => e.SDiaChi)
                .HasMaxLength(255)
                .HasColumnName("sDiaChi");
            entity.Property(e => e.SDienThoai)
                .HasMaxLength(20)
                .HasColumnName("sDienThoai");
            entity.Property(e => e.STenNcc)
                .HasMaxLength(200)
                .HasColumnName("sTenNCC");
        });

        modelBuilder.Entity<TblNhanVien>(entity =>
        {
            entity.HasKey(e => e.IMaNv);

            entity.ToTable("tblNhanVien");

            entity.HasIndex(e => e.SDienThoai, "UQ_tblNhanVien_sDienThoai").IsUnique();

            entity.Property(e => e.IMaNv).HasColumnName("iMaNV");
            entity.Property(e => e.IMaVaiTro).HasColumnName("iMaVaiTro");
            entity.Property(e => e.SDienThoai)
                .HasMaxLength(20)
                .HasColumnName("sDienThoai");
            entity.Property(e => e.SGioiTinh)
                .HasMaxLength(20)
                .HasColumnName("sGioiTinh");
            entity.Property(e => e.STenNv)
                .HasMaxLength(150)
                .HasColumnName("sTenNV");

            entity.HasOne(d => d.IMaVaiTroNavigation).WithMany(p => p.TblNhanViens)
                .HasForeignKey(d => d.IMaVaiTro)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblNhanVien_tblVaiTro");
        });

        modelBuilder.Entity<TblPhieuNhap>(entity =>
        {
            entity.HasKey(e => e.IMaPn);

            entity.ToTable("tblPhieuNhap");

            entity.Property(e => e.IMaPn).HasColumnName("iMaPN");
            entity.Property(e => e.DNgayNhap)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("dNgayNhap");
            entity.Property(e => e.IMaNcc).HasColumnName("iMaNCC");
            entity.Property(e => e.IMaNv).HasColumnName("iMaNV");

            entity.HasOne(d => d.IMaNccNavigation).WithMany(p => p.TblPhieuNhaps)
                .HasForeignKey(d => d.IMaNcc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPhieuNhap_tblNhaCungCap");

            entity.HasOne(d => d.IMaNvNavigation).WithMany(p => p.TblPhieuNhaps)
                .HasForeignKey(d => d.IMaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblPhieuNhap_tblNhanVien");
        });

        modelBuilder.Entity<TblQuanAo>(entity =>
        {
            entity.HasKey(e => e.IMaSp);

            entity.ToTable("tblQuanAo");

            entity.Property(e => e.IMaSp)
                .ValueGeneratedNever()
                .HasColumnName("iMaSP");
            entity.Property(e => e.SChatLieu)
                .HasMaxLength(100)
                .HasColumnName("sChatLieu");
            entity.Property(e => e.SKieuDang)
                .HasMaxLength(100)
                .HasColumnName("sKieuDang");

            entity.HasOne(d => d.IMaSpNavigation).WithOne(p => p.TblQuanAo)
                .HasForeignKey<TblQuanAo>(d => d.IMaSp)
                .HasConstraintName("FK_tblQuanAo_tblSanPham");
        });

        modelBuilder.Entity<TblSanPham>(entity =>
        {
            entity.HasKey(e => e.IMaSp);

            entity.ToTable("tblSanPham");

            entity.Property(e => e.IMaSp).HasColumnName("iMaSP");
            entity.Property(e => e.DNgayHetHan).HasColumnName("dNgayHetHan");
            entity.Property(e => e.FGiaBan)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fGiaBan");
            entity.Property(e => e.FGiaNhap)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fGiaNhap");
            entity.Property(e => e.IMaNcc).HasColumnName("iMaNCC");
            entity.Property(e => e.ISoLuongTon).HasColumnName("iSoLuongTon");
            entity.Property(e => e.SMoTa).HasColumnName("sMoTa");
            entity.Property(e => e.STenSp)
                .HasMaxLength(200)
                .HasColumnName("sTenSP");
            entity.Property(e => e.SXuatXu)
                .HasMaxLength(100)
                .HasColumnName("sXuatXu");

            entity.HasOne(d => d.IMaNccNavigation).WithMany(p => p.TblSanPhams)
                .HasForeignKey(d => d.IMaNcc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblSanPham_tblNhaCungCap");
        });

        modelBuilder.Entity<TblSanPhamTblKichThuoc>(entity =>
        {
            entity.HasKey(e => new { e.IMaSp, e.IMaSize });

            entity.ToTable("tblSanPham_tblKichThuoc");

            entity.Property(e => e.IMaSp).HasColumnName("iMaSP");
            entity.Property(e => e.IMaSize).HasColumnName("iMaSize");

            entity.HasOne(d => d.IMaSizeNavigation).WithMany(p => p.TblSanPhamTblKichThuocs)
                .HasForeignKey(d => d.IMaSize)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SP_Size_Size");

            entity.HasOne(d => d.IMaSpNavigation).WithMany(p => p.TblSanPhamTblKichThuocs)
                .HasForeignKey(d => d.IMaSp)
                .HasConstraintName("FK_SP_Size_SanPham");
        });

        modelBuilder.Entity<TblSanPhamTblMauSac>(entity =>
        {
            entity.HasKey(e => new { e.IMaSp, e.IMaMau });

            entity.ToTable("tblSanPham_tblMauSac");

            entity.Property(e => e.IMaSp).HasColumnName("iMaSP");
            entity.Property(e => e.IMaMau).HasColumnName("iMaMau");

            entity.HasOne(d => d.IMaMauNavigation).WithMany(p => p.TblSanPhamTblMauSacs)
                .HasForeignKey(d => d.IMaMau)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SP_Mau_MauSac");

            entity.HasOne(d => d.IMaSpNavigation).WithMany(p => p.TblSanPhamTblMauSacs)
                .HasForeignKey(d => d.IMaSp)
                .HasConstraintName("FK_SP_Mau_SanPham");
        });

        modelBuilder.Entity<TblTaiKhoan>(entity =>
        {
            entity.HasKey(e => e.STenDangNhap);

            entity.ToTable("tblTaiKhoan");

            entity.HasIndex(e => e.IMaNv, "UQ_tblTaiKhoan_iMaNV").IsUnique();

            entity.Property(e => e.STenDangNhap)
                .HasMaxLength(50)
                .HasColumnName("sTenDangNhap");
            entity.Property(e => e.IMaNv).HasColumnName("iMaNV");
            entity.Property(e => e.SMatKhau)
                .HasMaxLength(255)
                .HasColumnName("sMatKhau");

            entity.HasOne(d => d.IMaNvNavigation).WithOne(p => p.TblTaiKhoan)
                .HasForeignKey<TblTaiKhoan>(d => d.IMaNv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_tblTaiKhoan_tblNhanVien");
        });

        modelBuilder.Entity<TblTonKhoChiTiet>(entity =>
        {
            entity.HasKey(e => new { e.IMaSp, e.IMaSize, e.IMaMau });

            entity.ToTable("tblTonKhoChiTiet", tb => tb.HasTrigger("trg_TonKhoChiTiet_CapNhatTongSanPham"));

            entity.Property(e => e.IMaSp).HasColumnName("iMaSP");
            entity.Property(e => e.IMaSize).HasColumnName("iMaSize");
            entity.Property(e => e.IMaMau).HasColumnName("iMaMau");
            entity.Property(e => e.ISoLuongTon).HasColumnName("iSoLuongTon");

            entity.HasOne(d => d.TblSanPhamTblMauSac).WithMany(p => p.TblTonKhoChiTiets)
                .HasForeignKey(d => new { d.IMaSp, d.IMaMau })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TonKho_SP_Mau");

            entity.HasOne(d => d.TblSanPhamTblKichThuoc).WithMany(p => p.TblTonKhoChiTiets)
                .HasForeignKey(d => new { d.IMaSp, d.IMaSize })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TonKho_SP_Size");
        });

        modelBuilder.Entity<TblVaiTro>(entity =>
        {
            entity.HasKey(e => e.IMaVaiTro);

            entity.ToTable("tblVaiTro");

            entity.HasIndex(e => e.STenVaiTro, "UQ_tblVaiTro_sTenVaiTro").IsUnique();

            entity.Property(e => e.IMaVaiTro)
                .ValueGeneratedNever()
                .HasColumnName("iMaVaiTro");
            entity.Property(e => e.STenVaiTro)
                .HasMaxLength(100)
                .HasColumnName("sTenVaiTro");
        });

        modelBuilder.Entity<VBaoCaoDoanhThu>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_BaoCaoDoanhThu");

            entity.Property(e => e.DNgay).HasColumnName("dNgay");
            entity.Property(e => e.FDoanhThuThucThu)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fDoanhThuThucThu");
            entity.Property(e => e.FDoanhThuTruocGiam)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("fDoanhThuTruocGiam");
            entity.Property(e => e.FGiaVon)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fGiaVon");
            entity.Property(e => e.FLoiNhuanGop)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fLoiNhuanGop");
            entity.Property(e => e.IMaHd).HasColumnName("iMaHD");
        });

        modelBuilder.Entity<VInHoaDon>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_InHoaDon");

            entity.Property(e => e.DNgayLap)
                .HasPrecision(0)
                .HasColumnName("dNgayLap");
            entity.Property(e => e.FDonGia)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fDonGia");
            entity.Property(e => e.FGiamGia)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fGiamGia");
            entity.Property(e => e.FThanhTienGoc)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fThanhTienGoc");
            entity.Property(e => e.FThanhTienSauGiam10)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fThanhTienSauGiam10");
            entity.Property(e => e.FThanhToan)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fThanhToan");
            entity.Property(e => e.FTienGiam10)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fTienGiam10");
            entity.Property(e => e.FTienKhachTra)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fTienKhachTra");
            entity.Property(e => e.FTienTraLai)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fTienTraLai");
            entity.Property(e => e.FTongTien)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("fTongTien");
            entity.Property(e => e.IMaHd).HasColumnName("iMaHD");
            entity.Property(e => e.IMaKh).HasColumnName("iMaKH");
            entity.Property(e => e.IMaNv).HasColumnName("iMaNV");
            entity.Property(e => e.IMaSp).HasColumnName("iMaSP");
            entity.Property(e => e.ISoLuong).HasColumnName("iSoLuong");
            entity.Property(e => e.SDienThoaiKh)
                .HasMaxLength(20)
                .HasColumnName("sDienThoaiKH");
            entity.Property(e => e.SPhuongThucTt)
                .HasMaxLength(50)
                .HasColumnName("sPhuongThucTT");
            entity.Property(e => e.STenKh)
                .HasMaxLength(150)
                .HasColumnName("sTenKH");
            entity.Property(e => e.STenMau)
                .HasMaxLength(50)
                .HasColumnName("sTenMau");
            entity.Property(e => e.STenNv)
                .HasMaxLength(150)
                .HasColumnName("sTenNV");
            entity.Property(e => e.STenSize)
                .HasMaxLength(20)
                .HasColumnName("sTenSize");
            entity.Property(e => e.STenSp)
                .HasMaxLength(200)
                .HasColumnName("sTenSP");
        });

        modelBuilder.Entity<VThi>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_thi");

            entity.Property(e => e.DNgayHetHan).HasColumnName("dNgayHetHan");
            entity.Property(e => e.IMaMau).HasColumnName("iMaMau");
            entity.Property(e => e.IMaSize).HasColumnName("iMaSize");
            entity.Property(e => e.IMaSp).HasColumnName("iMaSP");
            entity.Property(e => e.ISoLuongTon).HasColumnName("iSoLuongTon");
            entity.Property(e => e.STenMau)
                .HasMaxLength(50)
                .HasColumnName("sTenMau");
            entity.Property(e => e.STenSize)
                .HasMaxLength(20)
                .HasColumnName("sTenSize");
            entity.Property(e => e.STenSp)
                .HasMaxLength(200)
                .HasColumnName("sTenSP");
            entity.Property(e => e.STrangThaiHanDung)
                .HasMaxLength(13)
                .HasColumnName("sTrangThaiHanDung");
            entity.Property(e => e.SXuatXu)
                .HasMaxLength(100)
                .HasColumnName("sXuatXu");
        });

        modelBuilder.Entity<VTonKho>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_TonKho");

            entity.Property(e => e.FTongBan).HasColumnName("fTongBan");
            entity.Property(e => e.FTongNhap).HasColumnName("fTongNhap");
            entity.Property(e => e.IMaMau).HasColumnName("iMaMau");
            entity.Property(e => e.IMaSize).HasColumnName("iMaSize");
            entity.Property(e => e.IMaSp).HasColumnName("iMaSP");
            entity.Property(e => e.ISoLuongTon).HasColumnName("iSoLuongTon");
            entity.Property(e => e.STenMau)
                .HasMaxLength(50)
                .HasColumnName("sTenMau");
            entity.Property(e => e.STenSize)
                .HasMaxLength(20)
                .HasColumnName("sTenSize");
            entity.Property(e => e.STenSp)
                .HasMaxLength(200)
                .HasColumnName("sTenSP");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
