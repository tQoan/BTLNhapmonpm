-- ============================================================
-- DATABASE: QuanLyCuaHangTheThao
-- Script tao database day du (schema + triggers + views + seed)
-- Phuong phap: Database-First, EF Core 8
--
-- Pham vi: 3 chuc nang chinh
--   1. Ban hang          (Khach hang, Hoa don, Chi tiet hoa don)
--   2. Kho & San pham    (NCC, Kich thuoc, Mau sac, San pham, Ton kho, Phieu nhap)
--   3. Bao cao & Quan tri (Vai tro, Nhan vien, Tai khoan, View bao cao)
--
-- Cap nhat: bo module Lich lam viec / Ca lam viec,
--           chuan hoa vai tro con 2 muc: Admin, Ban hang (co dau).
--
-- CANH BAO: script DROP DATABASE QuanLyCuaHangTheThao truoc khi tao lai.
--           Chi chay tren may dev / database trong. Muon giu du lieu cu
--           thi dung script nang cap rieng trong thu muc scripts/.
-- ============================================================

-- ============================================================
-- TRANG THAI: HOAN THANH (ban 1 - 18/11/2026)
-- Nguoi thuc hien: Dao Quoc Toan
-- Doi chieu voi ma nguon: 3 chuc nang (Ban hang | Kho & San pham | Bao cao & Quan tri)
--
-- DA XONG
--   [x] 18 bang: vai tro, nhan vien, tai khoan, khach hang, NCC, size, mau,
--       san pham, 2 bang bien the, 3 bang loai SP, ton kho, hoa don,
--       chi tiet hoa don, phieu nhap, chi tiet phieu nhap
--   [x] 22 khoa ngoai (FK) rang buoc toan ven du lieu
--   [x] 3 trigger: tru ton khi ban, cong ton khi nhap, dong bo tong ton san pham
--   [x] 4 view: v_TonKho, v_thi, v_BaoCaoDoanhThu, v_InHoaDon
--   [x] 2 vai tro dung dau: Admin, Ban hang (khop [Authorize] trong code)
--   [x] Seed: admin/admin123, 5 size, 5 mau, 3 NCC, 3 khach hang mau
--   [x] Da bo module Lich lam viec / Ca lam viec (2 bang + 2 FK + seed ca)
--
-- CHUA LAM (de xuat dot sau)
--   [ ] Du lieu demo: san pham, phieu nhap, hoa don mau de dashboard co so lieu
--   [ ] Script nang cap (idempotent) cho database dang co du lieu that
--   [ ] Rang buoc CHECK cho so luong / don gia > 0
-- ============================================================

USE master;
GO

IF DB_ID('QuanLyCuaHangTheThao') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyCuaHangTheThao SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QuanLyCuaHangTheThao;
END
GO

CREATE DATABASE QuanLyCuaHangTheThao;
GO

USE QuanLyCuaHangTheThao;
GO

-- ============================================================
-- 1. TAO BANG
-- ============================================================

-- Vai tro
CREATE TABLE tblVaiTro (
    iMaVaiTro   INT PRIMARY KEY,
    sTenVaiTro  NVARCHAR(100) NOT NULL
);
CREATE UNIQUE INDEX UQ_tblVaiTro_sTenVaiTro ON tblVaiTro(sTenVaiTro);

-- Nhan vien
CREATE TABLE tblNhanVien (
    iMaNV       INT IDENTITY(1,1) PRIMARY KEY,
    sTenNV      NVARCHAR(150) NOT NULL,
    sGioiTinh   NVARCHAR(20)  NULL,
    sDienThoai  NVARCHAR(20)  NULL,
    iMaVaiTro   INT NOT NULL
);
CREATE UNIQUE INDEX UQ_tblNhanVien_sDienThoai ON tblNhanVien(sDienThoai) WHERE sDienThoai IS NOT NULL;

-- Tai khoan (dang nhap)
CREATE TABLE tblTaiKhoan (
    sTenDangNhap NVARCHAR(50)  PRIMARY KEY,
    sMatKhau     NVARCHAR(255) NOT NULL,
    iMaNV        INT NOT NULL
);
CREATE UNIQUE INDEX UQ_tblTaiKhoan_iMaNV ON tblTaiKhoan(iMaNV);

-- Khach hang
CREATE TABLE tblKhachHang (
    iMaKH       INT IDENTITY(1,1) PRIMARY KEY,
    sTenKH      NVARCHAR(150) NOT NULL,
    sDienThoai  NVARCHAR(20)  NULL,
    sDiaChi     NVARCHAR(255) NULL
);
CREATE UNIQUE INDEX UQ_tblKhachHang_sDienThoai ON tblKhachHang(sDienThoai) WHERE sDienThoai IS NOT NULL;

-- Nha cung cap
CREATE TABLE tblNhaCungCap (
    iMaNCC      INT IDENTITY(1,1) PRIMARY KEY,
    sTenNCC     NVARCHAR(200) NOT NULL,
    sDiaChi     NVARCHAR(255) NULL,
    sDienThoai  NVARCHAR(20)  NULL
);
CREATE UNIQUE INDEX UQ_tblNhaCungCap_sTenNCC  ON tblNhaCungCap(sTenNCC);
CREATE UNIQUE INDEX UQ_tblNhaCungCap_sDienThoai ON tblNhaCungCap(sDienThoai) WHERE sDienThoai IS NOT NULL;

-- Kich thuoc (size)
CREATE TABLE tblKichThuoc (
    iMaSize   INT IDENTITY(1,1) PRIMARY KEY,
    sTenSize  NVARCHAR(20) NOT NULL
);
CREATE UNIQUE INDEX UQ_tblKichThuoc_sTenSize ON tblKichThuoc(sTenSize);

-- Mau sac
CREATE TABLE tblMauSac (
    iMaMau   INT IDENTITY(1,1) PRIMARY KEY,
    sTenMau  NVARCHAR(50) NOT NULL
);
CREATE UNIQUE INDEX UQ_tblMauSac_sTenMau ON tblMauSac(sTenMau);

-- San pham
CREATE TABLE tblSanPham (
    iMaSP         INT IDENTITY(1,1) PRIMARY KEY,
    sTenSP        NVARCHAR(200) NOT NULL,
    iMaNCC        INT NOT NULL,
    sXuatXu       NVARCHAR(100) NULL,
    fGiaNhap      DECIMAL(18,2) NOT NULL,
    fGiaBan       DECIMAL(18,2) NOT NULL,
    sMoTa         NVARCHAR(MAX) NULL,
    iSoLuongTon   INT NOT NULL DEFAULT 0,
    dNgayHetHan   DATE NULL
);

-- SanPham - KichThuoc (join table)
CREATE TABLE tblSanPham_tblKichThuoc (
    iMaSP   INT NOT NULL,
    iMaSize INT NOT NULL,
    PRIMARY KEY (iMaSP, iMaSize)
);

-- SanPham - MauSac (join table)
CREATE TABLE tblSanPham_tblMauSac (
    iMaSP  INT NOT NULL,
    iMaMau INT NOT NULL,
    PRIMARY KEY (iMaSP, iMaMau)
);

-- Loai san pham: Quan ao
CREATE TABLE tblQuanAo (
    iMaSP      INT PRIMARY KEY,
    sChatLieu  NVARCHAR(100) NULL,
    sKieuDang  NVARCHAR(100) NULL
);

-- Loai san pham: Giay dep
CREATE TABLE tblGiayDep (
    iMaSP      INT PRIMARY KEY,
    sChatLieu  NVARCHAR(100) NULL,
    sLoaiDe    NVARCHAR(100) NULL
);

-- Loai san pham: Dung cu
CREATE TABLE tblDungCu (
    iMaSP      INT PRIMARY KEY,
    sChatLieu  NVARCHAR(100) NULL,
    sCongDung  NVARCHAR(200) NULL
);

-- Ton kho chi tiet (theo size x mau)
CREATE TABLE tblTonKhoChiTiet (
    iMaSP       INT NOT NULL,
    iMaSize     INT NOT NULL,
    iMaMau      INT NOT NULL,
    iSoLuongTon INT NOT NULL DEFAULT 0,
    PRIMARY KEY (iMaSP, iMaSize, iMaMau)
);

-- Hoa don
CREATE TABLE tblHoaDon (
    iMaHD          INT IDENTITY(1,1) PRIMARY KEY,
    dNgayLap       DATETIME2(0) NOT NULL DEFAULT (sysdatetime()),
    iMaKH          INT NULL,
    iMaNV          INT NOT NULL,
    fGiamGia       DECIMAL(18,2) NOT NULL DEFAULT 0,
    fTongTien      DECIMAL(18,2) NOT NULL DEFAULT 0,
    fThanhToan     DECIMAL(18,2) NOT NULL DEFAULT 0,
    sPhuongThucTT  NVARCHAR(50) NOT NULL DEFAULT N'Tiền mặt',
    fTienKhachTra  DECIMAL(18,2) NOT NULL DEFAULT 0,
    fTienTraLai    DECIMAL(18,2) NOT NULL DEFAULT 0
);

-- Chi tiet hoa don
CREATE TABLE tblChiTietHoaDon (
    iMaHD    INT NOT NULL,
    iMaSP    INT NOT NULL,
    iMaSize  INT NOT NULL,
    iMaMau   INT NOT NULL,
    iSoLuong INT NOT NULL,
    fDonGia  DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (iMaHD, iMaSP, iMaSize, iMaMau)
);

-- Phieu nhap
CREATE TABLE tblPhieuNhap (
    iMaPN      INT IDENTITY(1,1) PRIMARY KEY,
    dNgayNhap  DATETIME2(0) NOT NULL DEFAULT (sysdatetime()),
    iMaNCC     INT NOT NULL,
    iMaNV      INT NOT NULL
);

-- Chi tiet phieu nhap
CREATE TABLE tblChiTietPhieuNhap (
    iMaPN        INT NOT NULL,
    iMaSP        INT NOT NULL,
    iMaSize      INT NOT NULL,
    iMaMau       INT NOT NULL,
    iSoLuong     INT NOT NULL,
    fDonGiaNhap  DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (iMaPN, iMaSP, iMaSize, iMaMau)
);

-- ============================================================
-- 2. KHOA NGOAI (FOREIGN KEYS)
-- ============================================================

-- NhanVien -> VaiTro
ALTER TABLE tblNhanVien
    ADD CONSTRAINT FK_tblNhanVien_tblVaiTro
    FOREIGN KEY (iMaVaiTro) REFERENCES tblVaiTro(iMaVaiTro);

-- TaiKhoan -> NhanVien
ALTER TABLE tblTaiKhoan
    ADD CONSTRAINT FK_tblTaiKhoan_tblNhanVien
    FOREIGN KEY (iMaNV) REFERENCES tblNhanVien(iMaNV);

-- SanPham -> NhaCungCap
ALTER TABLE tblSanPham
    ADD CONSTRAINT FK_tblSanPham_tblNhaCungCap
    FOREIGN KEY (iMaNCC) REFERENCES tblNhaCungCap(iMaNCC);

-- SP_Size -> SanPham, KichThuoc
ALTER TABLE tblSanPham_tblKichThuoc
    ADD CONSTRAINT FK_SP_Size_SanPham FOREIGN KEY (iMaSP)   REFERENCES tblSanPham(iMaSP);
ALTER TABLE tblSanPham_tblKichThuoc
    ADD CONSTRAINT FK_SP_Size_Size     FOREIGN KEY (iMaSize) REFERENCES tblKichThuoc(iMaSize);

-- SP_Mau -> SanPham, MauSac
ALTER TABLE tblSanPham_tblMauSac
    ADD CONSTRAINT FK_SP_Mau_SanPham FOREIGN KEY (iMaSP)  REFERENCES tblSanPham(iMaSP);
ALTER TABLE tblSanPham_tblMauSac
    ADD CONSTRAINT FK_SP_Mau_MauSac  FOREIGN KEY (iMaMau) REFERENCES tblMauSac(iMaMau);

-- Loai san pham -> SanPham
ALTER TABLE tblQuanAo
    ADD CONSTRAINT FK_tblQuanAo_tblSanPham   FOREIGN KEY (iMaSP) REFERENCES tblSanPham(iMaSP);
ALTER TABLE tblGiayDep
    ADD CONSTRAINT FK_tblGiayDep_tblSanPham  FOREIGN KEY (iMaSP) REFERENCES tblSanPham(iMaSP);
ALTER TABLE tblDungCu
    ADD CONSTRAINT FK_tblDungCu_tblSanPham   FOREIGN KEY (iMaSP) REFERENCES tblSanPham(iMaSP);

-- TonKho -> SP_Size, SP_Mau
ALTER TABLE tblTonKhoChiTiet
    ADD CONSTRAINT FK_TonKho_SP_Size FOREIGN KEY (iMaSP, iMaSize) REFERENCES tblSanPham_tblKichThuoc(iMaSP, iMaSize);
ALTER TABLE tblTonKhoChiTiet
    ADD CONSTRAINT FK_TonKho_SP_Mau  FOREIGN KEY (iMaSP, iMaMau)  REFERENCES tblSanPham_tblMauSac(iMaSP, iMaMau);

-- HoaDon -> KhachHang, NhanVien
ALTER TABLE tblHoaDon
    ADD CONSTRAINT FK_tblHoaDon_tblKhachHang FOREIGN KEY (iMaKH) REFERENCES tblKhachHang(iMaKH);
ALTER TABLE tblHoaDon
    ADD CONSTRAINT FK_tblHoaDon_tblNhanVien  FOREIGN KEY (iMaNV) REFERENCES tblNhanVien(iMaNV);

-- ChiTietHoaDon -> HoaDon, SP_Size, SP_Mau
ALTER TABLE tblChiTietHoaDon
    ADD CONSTRAINT FK_CTHD_HoaDon  FOREIGN KEY (iMaHD)               REFERENCES tblHoaDon(iMaHD);
ALTER TABLE tblChiTietHoaDon
    ADD CONSTRAINT FK_CTHD_SP_Size FOREIGN KEY (iMaSP, iMaSize)      REFERENCES tblSanPham_tblKichThuoc(iMaSP, iMaSize);
ALTER TABLE tblChiTietHoaDon
    ADD CONSTRAINT FK_CTHD_SP_Mau  FOREIGN KEY (iMaSP, iMaMau)       REFERENCES tblSanPham_tblMauSac(iMaSP, iMaMau);

-- PhieuNhap -> NhaCungCap, NhanVien
ALTER TABLE tblPhieuNhap
    ADD CONSTRAINT FK_tblPhieuNhap_tblNhaCungCap FOREIGN KEY (iMaNCC) REFERENCES tblNhaCungCap(iMaNCC);
ALTER TABLE tblPhieuNhap
    ADD CONSTRAINT FK_tblPhieuNhap_tblNhanVien   FOREIGN KEY (iMaNV) REFERENCES tblNhanVien(iMaNV);

-- ChiTietPhieuNhap -> PhieuNhap, SP_Size, SP_Mau
ALTER TABLE tblChiTietPhieuNhap
    ADD CONSTRAINT FK_CTPN_PhieuNhap FOREIGN KEY (iMaPN)               REFERENCES tblPhieuNhap(iMaPN);
ALTER TABLE tblChiTietPhieuNhap
    ADD CONSTRAINT FK_CTPN_SP_Size   FOREIGN KEY (iMaSP, iMaSize)      REFERENCES tblSanPham_tblKichThuoc(iMaSP, iMaSize);
ALTER TABLE tblChiTietPhieuNhap
    ADD CONSTRAINT FK_CTPN_SP_Mau    FOREIGN KEY (iMaSP, iMaMau)       REFERENCES tblSanPham_tblMauSac(iMaSP, iMaMau);

-- ============================================================
-- 3. TRIGGERS
-- ============================================================

-- -------------------------------------------------------
-- Trigger 1: Kiem tra ton kho khi ban hang
-- Kiem tra ton du truoc khi insert chi tiet hoa don
-- va giam ton kho tuong ung
-- -------------------------------------------------------
GO
CREATE OR ALTER TRIGGER trg_ChiTietHoaDon_KiemTraTonVaCapNhat
ON tblChiTietHoaDon
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiem tra ton kho khong du
    IF EXISTS (
        SELECT 1
        FROM inserted i
        LEFT JOIN tblTonKhoChiTiet tk
            ON i.iMaSP = tk.iMaSP AND i.iMaSize = tk.iMaSize AND i.iMaMau = tk.iMaMau
        WHERE ISNULL(tk.iSoLuongTon, 0) < i.iSoLuong
    )
    BEGIN
        RAISERROR(N'So luong ton kho khong du de ban hang.', 16, 1);
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Giam ton kho
    UPDATE tk
    SET tk.iSoLuongTon = tk.iSoLuongTon - i.iSoLuong
    FROM tblTonKhoChiTiet tk
    INNER JOIN inserted i
        ON tk.iMaSP = i.iMaSP AND tk.iMaSize = i.iMaSize AND tk.iMaMau = i.iMaMau;
END
GO

-- -------------------------------------------------------
-- Trigger 2: Cap nhat ton kho khi nhap hang
-- Tang ton kho khi insert chi tiet phieu nhap
-- -------------------------------------------------------
GO
CREATE OR ALTER TRIGGER trg_ChiTietPhieuNhap_CapNhatTonKho
ON tblChiTietPhieuNhap
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Tang ton kho
    UPDATE tk
    SET tk.iSoLuongTon = tk.iSoLuongTon + i.iSoLuong
    FROM tblTonKhoChiTiet tk
    INNER JOIN inserted i
        ON tk.iMaSP = i.iMaSP AND tk.iMaSize = i.iMaSize AND tk.iMaMau = i.iMaMau;
END
GO

-- -------------------------------------------------------
-- Trigger 3: Cap nhat tong so luong ton tren tblSanPham
-- Khi ton kho chi tiet thay doi, cap nhat lai iSoLuongTon
-- -------------------------------------------------------
GO
CREATE OR ALTER TRIGGER trg_TonKhoChiTiet_CapNhatTongSanPham
ON tblTonKhoChiTiet
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- Cap nhat cho cac san pham bi anh huong
    UPDATE sp
    SET sp.iSoLuongTon = ISNULL(sub.Tong, 0)
    FROM tblSanPham sp
    INNER JOIN (
        SELECT iMaSP, SUM(iSoLuongTon) AS Tong
        FROM tblTonKhoChiTiet
        GROUP BY iMaSP
    ) sub ON sp.iMaSP = sub.iMaSP
    WHERE sp.iMaSP IN (
        SELECT iMaSP FROM inserted
        UNION
        SELECT iMaSP FROM deleted
    );
END
GO

-- ============================================================
-- 4. VIEWS
-- ============================================================

-- -------------------------------------------------------
-- View: Ton kho theo san pham x size x mau
-- -------------------------------------------------------
GO
CREATE OR ALTER VIEW v_TonKho AS
SELECT
    sp.iMaSP,
    sp.sTenSP,
    kt.iMaSize,
    kt.sTenSize,
    ms.iMaMau,
    ms.sTenMau,
    ISNULL(pn.TongNhap, 0)  AS fTongNhap,
    ISNULL(hd.TongBan, 0)   AS fTongBan,
    tk.iSoLuongTon
FROM tblTonKhoChiTiet tk
INNER JOIN tblSanPham sp       ON tk.iMaSP  = sp.iMaSP
INNER JOIN tblKichThuoc kt     ON tk.iMaSize = kt.iMaSize
INNER JOIN tblMauSac ms        ON tk.iMaMau  = ms.iMaMau
LEFT JOIN (
    SELECT iMaSP, iMaSize, iMaMau, SUM(iSoLuong) AS TongNhap
    FROM tblChiTietPhieuNhap
    GROUP BY iMaSP, iMaSize, iMaMau
) pn ON tk.iMaSP = pn.iMaSP AND tk.iMaSize = pn.iMaSize AND tk.iMaMau = pn.iMaMau
LEFT JOIN (
    SELECT iMaSP, iMaSize, iMaMau, SUM(iSoLuong) AS TongBan
    FROM tblChiTietHoaDon
    GROUP BY iMaSP, iMaSize, iMaMau
) hd ON tk.iMaSP = hd.iMaSP AND tk.iMaSize = hd.iMaSize AND tk.iMaMau = hd.iMaMau;
GO

-- -------------------------------------------------------
-- View: Han dung san pham
-- -------------------------------------------------------
GO
CREATE OR ALTER VIEW v_thi AS
SELECT
    sp.iMaSP,
    sp.sTenSP,
    sp.sXuatXu,
    kt.iMaSize,
    kt.sTenSize,
    ms.iMaMau,
    ms.sTenMau,
    tk.iSoLuongTon,
    sp.dNgayHetHan,
    CASE
        WHEN sp.dNgayHetHan IS NULL THEN N'Khong co han'
        WHEN sp.dNgayHetHan < CAST(GETDATE() AS DATE) THEN N'Da het han'
        WHEN sp.dNgayHetHan <= DATEADD(DAY, 30, GETDATE()) THEN N'Sap het han'
        ELSE N'Con han'
    END AS sTrangThaiHanDung
FROM tblTonKhoChiTiet tk
INNER JOIN tblSanPham sp   ON tk.iMaSP  = sp.iMaSP
INNER JOIN tblKichThuoc kt ON tk.iMaSize = kt.iMaSize
INNER JOIN tblMauSac ms    ON tk.iMaMau  = ms.iMaMau;
GO

-- -------------------------------------------------------
-- View: Bao cao doanh thu
-- -------------------------------------------------------
GO
CREATE OR ALTER VIEW v_BaoCaoDoanhThu AS
SELECT
    CAST(hd.dNgayLap AS DATE)      AS dNgay,
    hd.iMaHD,
    hd.fTongTien                   AS fDoanhThuTruocGiam,
    hd.fThanhToan                  AS fDoanhThuThucThu,
    ISNULL(von.GiaVon, 0)          AS fGiaVon,
    (hd.fThanhToan - ISNULL(von.GiaVon, 0)) AS fLoiNhuanGop
FROM tblHoaDon hd
LEFT JOIN (
    SELECT cthd.iMaHD, SUM(cthd.iSoLuong * sp.fGiaNhap) AS GiaVon
    FROM tblChiTietHoaDon cthd
    INNER JOIN tblSanPham sp ON cthd.iMaSP = sp.iMaSP
    GROUP BY cthd.iMaHD
) von ON hd.iMaHD = von.iMaHD;
GO

-- -------------------------------------------------------
-- View: In hoa don
-- -------------------------------------------------------
GO
CREATE OR ALTER VIEW v_InHoaDon AS
SELECT
    hd.iMaHD,
    hd.dNgayLap,
    hd.iMaKH,
    kh.sTenKH,
    kh.sDienThoai    AS sDienThoaiKH,
    hd.iMaNV,
    nv.sTenNV,
    cthd.iMaSP,
    sp.sTenSP,
    kt.sTenSize,
    ms.sTenMau,
    cthd.iSoLuong,
    cthd.fDonGia,
    (cthd.iSoLuong * cthd.fDonGia)                                        AS fThanhTienGoc,
    (cthd.iSoLuong * cthd.fDonGia * (1 - hd.fGiamGia / 100))             AS fThanhTienSauGiam10,
    (cthd.iSoLuong * cthd.fDonGia * hd.fGiamGia / 100)                   AS fTienGiam10,
    hd.fGiamGia,
    hd.fTongTien,
    hd.fThanhToan,
    hd.sPhuongThucTT,
    hd.fTienKhachTra,
    hd.fTienTraLai
FROM tblHoaDon hd
INNER JOIN tblChiTietHoaDon cthd ON hd.iMaHD = cthd.iMaHD
INNER JOIN tblSanPham sp         ON cthd.iMaSP = sp.iMaSP
INNER JOIN tblSanPham_tblKichThuoc spkt ON cthd.iMaSP = spkt.iMaSP AND cthd.iMaSize = spkt.iMaSize
INNER JOIN tblKichThuoc kt       ON spkt.iMaSize = kt.iMaSize
INNER JOIN tblSanPham_tblMauSac spms ON cthd.iMaSP = spms.iMaSP AND cthd.iMaMau = spms.iMaMau
INNER JOIN tblMauSac ms          ON spms.iMaMau = ms.iMaMau
LEFT JOIN tblNhanVien nv         ON hd.iMaNV = nv.iMaNV
LEFT JOIN tblKhachHang kh        ON hd.iMaKH = kh.iMaKH;
GO

-- ============================================================
-- 5. SEED DATA
-- ============================================================

-- Vai tro (2 muc, dung dau de khop [Authorize] trong code)
INSERT INTO tblVaiTro (iMaVaiTro, sTenVaiTro) VALUES
(1, N'Admin'),
(2, N'Bán hàng');

-- Nhan vien mac dinh (admin)
INSERT INTO tblNhanVien (sTenNV, sGioiTinh, sDienThoai, iMaVaiTro) VALUES
(N'Admin', N'Nam', N'0900000000', 1);

-- Tai khoan admin: admin / admin123
-- Mat khau PBKDF2:100000:... duoc tao boi PasswordHasher.Hash("admin123")
-- Neu khong co hash, de trong va Program.cs se upgrade khi startup
INSERT INTO tblTaiKhoan (sTenDangNhap, sMatKhau, iMaNV) VALUES
(N'admin', N'admin123', 1);

-- Kich thuoc (size)
INSERT INTO tblKichThuoc (sTenSize) VALUES
(N'S'), (N'M'), (N'L'), (N'XL'), (N'XXL');

-- Mau sac
INSERT INTO tblMauSac (sTenMau) VALUES
(N'Den'), (N'Trang'), (N'Do'), (N'Xanh duong'), (N'Xanh la');

-- Nha cung cap mau
INSERT INTO tblNhaCungCap (sTenNCC, sDiaChi, sDienThoai) VALUES
(N'Nike Vietnam',   N'Quan 1, TP.HCM',  N'02812345678'),
(N'Adidas VN',      N'Quan 3, TP.HCM',  N'02887654321'),
(N'SportMaster',    N'Ha Noi',           N'02411112222');

-- Khach hang mau
INSERT INTO tblKhachHang (sTenKH, sDienThoai, sDiaChi) VALUES
(N'Nguyen Van A',   N'0912345678', N'Quan 1, TP.HCM'),
(N'Tran Thi B',     N'0987654321', N'Quan 7, TP.HCM'),
(N'Khach le',       NULL,          NULL);

PRINT N'Tao database thanh cong!';
PRINT N'Tai khoan mac dinh: admin / admin123';
GO
