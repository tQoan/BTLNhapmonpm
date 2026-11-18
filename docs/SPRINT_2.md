# Sprint 2 - Quan Ly Cua Hang The Thao

> Thoi gian: Tuần 2
> Sprint Goal: "CRUD day du cho HoaDon/NhapHang + Fixes + Unit tests"
> Tong story points: 10 SP + 1 bonus fix

---

## Danh sach Task

| ID  | Task                                                                  | Owner | SP | Status | Notes                                     |
| --- | --------------------------------------------------------------------- | ----- | -- | ------ | ----------------------------------------- |
| F1  | Xoa hoa don (HoaDon Delete) - Admin only                             | Dev1  | 3  | Done   | Hoan tra ton kho khi xoa                  |
| F2  | Xoa phieu nhap (NhapHang Delete) + undo ton kho                      | Dev2  | 3  | Done   | Tru ton kho da cong khi xoa               |
| B2  | Xoa HomeController dead scaffold (Index, Privacy)                    | Dev1  | 1  | Done   | Giữ lai Error action cho exception handler |
| B4  | Fix Program.cs lazy upgrade password                                 | Dev2  | 2  | Done   | Chi load accounts can upgrade             |
| B6  | Xoa _LandingLayout.cshtml dead reference (window.foldcraft)          | Dev2  | 1  | Done   |                                          |
| T1  | Unit test cho PasswordHasher                                         | Dev1  | 3  | Done   | 7 tests pass; phat hien + fix bug malformed hash |

## Bonus Bug phat hien boi T1

| Bug  | Mo ta                                                        | Fix                                                                  |
| ---- | ------------------------------------------------------------ | -------------------------------------------------------------------- |
| P1   | `PasswordHasher.Verify` nem `FormatException` voi hash loi    | `PasswordHasher.cs:28-39` - dung `int.TryParse` + try/catch base64 |

## Chi tiet thuc hien

### F1: HoaDon Delete
- `HoaDonController.cs` - them action `Delete` (POST, `[Authorize(Roles="Admin")]`)
- Logic: hoan tra ton kho (cong lai `iSoLuongTon`) cho tung dong chi tiet truoc khi xoa
- `Views/HoaDon/Index.cshtml` - them nut "Xoa" (chi Admin thay duoc) voi confirm form
- Log warning khi xoa hoa don

### F2: NhapHang Delete
- `NhapHangController.cs` - them action `Delete` (POST, `[Authorize(Roles="Admin")]`)
- Logic: tru bot ton kho (undo trigger INSERT) cho tung dong truoc khi xoa
- `Views/NhapHang/Index.cshtml` - them nut "Xoa" (chi Admin)
- Them `ILogger<NhapHangController>` (phu vao B7 Sprint 1)
- Log warning khi xoa phieu nhap

### B2: HomeController
- `HomeController.cs` - xoa action `Index` va `Privacy`
- Xoa thu muc `Views/Home/` (Index.cshtml, Privacy.cshtml)
- GIU LAI action `Error` + `Views/Shared/Error.cshtml` vi `Program.cs:33` dung `/Home/Error`

### B4: Program.cs lazy upgrade
- Truoc: `db.TblTaiKhoans.ToList()` load toan bo accounts
- Sau: `db.TblTaiKhoans.Where(a => !a.SMatKhau.StartsWith("PBKDF2:"))` - chi load can upgrade
- `db.SaveChanges()` chi goi khi co account can upgrade

### B6: _LandingLayout
- Xoa khoi layout:
```html
<script>
    window.foldcraft = typeof window.foldcraft !== "undefined" ? window.foldcraft : {};
</script>
```

### T1: QuanLyCuaHangTheThao.Tests
- Tao project xunit moi `QuanLyCuaHangTheThao.Tests`
- Them vao solution
- 7 test: Hash format, hash khac nhau voi cung password, verify dung/sai, legacy plaintext, empty/null, malformed hash
- Phat hien bug: Verify nem FormatException -> da fix

---

## Definition of Done (Sprint 2)

- [x] `dotnet build QuanLyCuaHangTheThao.sln` - 0 warning, 0 error
- [x] `dotnet test` - 7/7 pass
- [ ] Test manual: xoa hoa don/phieu nhap hoan tra ton kho dung
