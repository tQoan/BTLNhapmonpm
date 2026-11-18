# Sprint 1 - Quan Ly Cua Hang The Thao

> Thoi gian: Tuần 1 (08/09/2026 - 14/09/2026)
> Sprint Goal: "Codebase sach + Database reproducible + POS khong loi variant"
> Tong story points: 9 SP

---

## Danh sach Task

| ID  | Task                                                               | Owner | SP | Status     | Notes                                    |
| --- | ------------------------------------------------------------------ | ----- | -- | ---------- | ---------------------------------------- |
| B1  | Xoa `frontend/` template dead code                                | Dev1  | 1  | In Progress | Xoa toan bo frontend/, giu src/ tham khao |
| B3  | Tao `database.sql` (schema + triggers + views + seed data)        | Dev1  | 3  | Todo       | Script tao DB day du de recreate         |
| B5  | Fix variant key collision trong `BanHangController`               | Dev2  | 2  | Todo       | Thay size*1000+mau bang tuple (int,int)  |
| F3  | Validation duplicate SĐT Khach hang                              | Dev1  | 2  | Todo       | Kiem tra truoc khi save                  |
| B7  | Them ILogger cho BanHangController, HoaDonController, AccountController | Dev2 | 1 | Todo    | Ghi log loi va thong tin quan trong      |

---

## B1: Xoa frontend/ template dead code

**Mo ta:** Thu muc `frontend/` chua template landing page "Foldcraft/LinkFlow" khong lien quan den he thong.
- Package name: `linkflow-frontend`
- Khong co ket noi API backend
- Chua `node_modules/` (tron lan)

**Thuc hien:**
```
Xoa toan bo thu muc frontend/
Giữ lại App.tsx, BoomerangVideoBg.tsx o dau neu can tham khao sau nay
```

**DoD:**
- [ ] Thu muc `frontend/` da xoa hoan toan
- [ ] `dotnet build` thanh cong
- [ ] Khong anh huong den backend

---

## B3: Tao database.sql

**Mo ta:** Tao file SQL chua day du schema de tao lai database tu dau.

**Noi dung script:**
```sql
-- 1. Tao database
CREATE DATABASE QuanLyCuaHangTheThao;

-- 2. Tao bang
-- tblVaiTro, tblNhanVien, tblTaiKhoan, tblKhachHang, tblNhaCungCap
-- tblKichThuoc, tblMauSac, tblSanPham
-- tblSanPham_tblKichThuoc, tblSanPham_tblMauSac
-- tblQuanAo, tblGiayDep, tblDungCu
-- tblTonKhoChiTiet
-- tblHoaDon, tblChiTietHoaDon
-- tblPhieuNhap, tblChiTietPhieuNhap

-- 3. Tao trigger
-- trg_ChiTietHoaDon_KiemTraTonVaCapNhat
-- trg_ChiTietPhieuNhap_CapNhatTonKho
-- trg_TonKhoChiTiet_CapNhatTongSanPham

-- 4. Tao view
-- v_TonKho, v_thi, v_BaoCaoDoanhThu, v_InHoaDon

-- 5. Seed data
-- Vai tro: Admin, Ban hang, Ke toan
-- Nhan vien mac dinh
-- Tai khoan admin (mat khau hash PBKDF2)
-- Kich thuoc mau: S, M, L, XL, XXL
-- Mau sac: Den, Trang, Do, Xanh...
```

**DoD:**
- [ ] File `database.sql` tao duoc database day du
- [ ] Triggers hoat dong dung (kiem tra stock, cap nhat ton kho)
- [ ] Views tra du lieu dung
- [ ] Seed data du (1 admin account, vai tro, size mau co ban)
- [ ] Script chay duoc tren SQL Server moi

---

## B5: Fix variant key collision

**Mo ta:** Trong `BanHangController.cs:130`, dictionary key duoc tinh bang `size * 1000 + mau`.
Neu `IMaSize > 999` se bi collision voi `IMaMau`.

**Noi dung fix:**

`BanHangController.cs` dong 122-138:
```sharp
// TRUOC:
var tkMap = tonKho.Where(t => t.IMaSp == sp.IMaSp)
    .ToDictionary(t => t.IMaSize * 1000 + t.IMaMau, t => t.ISoLuongTon);

// SAU:
var tkMap = tonKho.Where(t => t.IMaSp == sp.IMaSp)
    .ToDictionary(t => (t.IMaSize, t.IMaMau), t => t.ISoLuongTon);
```

Cung fix o `TonKhoController.cs` neu co.

**DoD:**
- [ ] `BanHangController` dung tuple key
- [ ] `dotnet build` thanh cong
- [ ] Ban hang hoat dong dung voi size > 999

---

## F3: Validation duplicate SĐT Khach hang

**Mo ta:** Hien tai co `UQ_tblKhachHang_sDienThoai` tren DB nhung khong validate o FE.
Neu trung SĐT se loi EF Exception thay vi thong bao dep.

**Thuc hien:**

`KhachHangController.cs` - them validation o Create va Edit:
```sharp
// Create - kiem tra truoc khi save
if (await _db.TblKhachHangs.AnyAsync(x => x.SDienThoai == model.SDienThoai))
{
    ModelState.AddModelError("SDienThoai", "So dien thoai da ton tai.");
    return View(model);
}

// Edit - kiem tra truoc khi save (loai tru chinh minh)
if (await _db.TblKhachHangs.AnyAsync(x => x.SDienThoai == model.SDienThoai && x.IMaKh != model.IMaKh))
{
    ModelState.AddModelError("SDienThoai", "So dien thoai da ton tai.");
    return View(model);
}
```

**DoD:**
- [ ] Create Khach hang: bao loi neu trung SĐT
- [ ] Edit Khach hang: bao loi neu trung SĐT (loai tru chinh minh)
- [ ] Message loi ro bang tieng Viet

---

## B7: Them ILogger cho Controllers

**Mo ta:** Them ILogger<T> vao constructor cua cac Controller de ghi log loi va thong tin.

**Thuc hien:**

Moi controller them:
```sharp
private readonly ILogger<XxxController> _logger;

public XxxController(QuanLyCuaHangTheThaoContext db, ILogger<XxxController> logger)
{
    _db = db;
    _logger = logger;
}
```

Sau do su dung:
```sharp
_logger.LogInformation("Tao hoa don #{MaHD}", hd.IMaHd);
_logger.LogWarning("Ton kho khong du cho san pham {MaSP}", line.IMaSp);
_logger.LogError(ex, "Loi khi luu hoa don");
```

**Ap dung cho (Sprint 1):** BanHangController, HoaDonController, AccountController

**DoD:**
- [ ] 3 controllers co ILogger
- [ ] Log o cac diem quan trong: login, tao HD, loi DB
- [ ] `dotnet build` thanh cong

---

## Daily Standup Log

| Ngay  | Nguoi | Hom qua          | Hom nay             | Blocked? |
| ----- | ----- | ---------------- | ------------------- | -------- |
| 08/09 | Dev1  | -                | Tao backlog, Sprint plan | Khong |
| 08/09 | Dev2  | -                | Doc hieu du an       | Khong    |

---

## Sprint Review (Thu 6)

**Demo:**
- [ ] `frontend/` da xoa
- [ ] `database.sql` tao duoc DB
- [ ] POS ban hang khong loi variant
- [ ] Khach hang validate SĐT
- [ ] 3 controllers co ILogger

**Retrospective:**
- Lam tot: ...
- Can cai thien: ...
- Hanh dong: ...
