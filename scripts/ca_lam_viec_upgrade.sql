-- ============================================================
-- NANG CAP: CHUC NANG QUAN LY CA LAM VIEC
-- Chay script nay tren database QuanLyCuaHangTheThao da co san
-- (nguoi moi tao DB thi database.sql da bao gom san cac bang nay).
-- Script idempotent: chay lai nhieu lan van an toan.
-- ============================================================

USE QuanLyCuaHangTheThao;
GO

-- ============================================================
-- 1. BANG DANH MUC CA
-- ============================================================
IF OBJECT_ID('tblCaLamViec', 'U') IS NULL
BEGIN
    CREATE TABLE tblCaLamViec (
        iMaCa        INT IDENTITY(1,1) PRIMARY KEY,
        sTenCa       NVARCHAR(100) NOT NULL,
        dGioBatDau   TIME         NOT NULL,
        dGioKetThuc  TIME         NOT NULL,
        CONSTRAINT CK_tblCaLamViec_Gio CHECK (dGioBatDau <> dGioKetThuc)
    );
    PRINT N'Da tao bang tblCaLamViec.';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'UQ_tblCaLamViec_sTenCa' AND object_id = OBJECT_ID('tblCaLamViec'))
    CREATE UNIQUE INDEX UQ_tblCaLamViec_sTenCa ON tblCaLamViec(sTenCa);
GO

-- ============================================================
-- 2. BANG LICH LAM VIEC
-- ============================================================
IF OBJECT_ID('tblLichLamViec', 'U') IS NULL
BEGIN
    CREATE TABLE tblLichLamViec (
        iMaLich    INT IDENTITY(1,1) PRIMARY KEY,
        iMaNV      INT NOT NULL,
        iMaCa      INT NOT NULL,
        dNgayLam   DATE NOT NULL,
        sGhiChu    NVARCHAR(255) NULL
    );
    PRINT N'Da tao bang tblLichLamViec.';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'UQ_tblLichLamViec_NV_Ngay' AND object_id = OBJECT_ID('tblLichLamViec'))
    CREATE UNIQUE INDEX UQ_tblLichLamViec_NV_Ngay ON tblLichLamViec(iMaNV, dNgayLam);
GO

-- ============================================================
-- 3. KHOA NGOAI
-- ============================================================
IF OBJECT_ID('tblLichLamViec', 'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_tblLichLamViec_tblNhanVien')
        ALTER TABLE tblLichLamViec
            ADD CONSTRAINT FK_tblLichLamViec_tblNhanVien
            FOREIGN KEY (iMaNV) REFERENCES tblNhanVien(iMaNV);

    IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_tblLichLamViec_tblCaLamViec')
        ALTER TABLE tblLichLamViec
            ADD CONSTRAINT FK_tblLichLamViec_tblCaLamViec
            FOREIGN KEY (iMaCa) REFERENCES tblCaLamViec(iMaCa);
END
GO

-- ============================================================
-- 4. DU LIEU MAC DINH
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM tblCaLamViec)
BEGIN
    INSERT INTO tblCaLamViec (sTenCa, dGioBatDau, dGioKetThuc) VALUES
    (N'Ca sang',  '07:00', '15:00'),
    (N'Ca chieu', '15:00', '23:00'),
    (N'Ca dem',   '23:00', '07:00');
    PRINT N'Da them 3 ca lam viec mac dinh.';
END
GO

PRINT N'Nang cap chuc nang quan ly ca lam viec thanh cong!';
GO