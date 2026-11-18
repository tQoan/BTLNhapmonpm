# Sprint 3 - Quan Ly Cua Hang The Thao

> Thoi gian: Tuần 3
> Sprint Goal: "Export bao cao CSV + ILogger day du + Test"
> Ghi chu: F5 (REST API) va T3 (deploy) CHUA lam trong pham vi nay vi can chay tren may that co SQL Server.

---

## Danh sach Task

| ID  | Task                                                                | Owner | SP | Status | Notes                                  |
| --- | ------------------------------------------------------------------- | ----- | -- | ------ | -------------------------------------- |
| F4  | Export bao cao CSV (BaoCaoController)                              | Dev1  | 5  | Done   | CSV UTF-8 BOM, Excel mo dung tieng Viet |
| B7  | Them ILogger cho cac controllers con lai                           | Dev2  | 2  | Done   | 9 controllers                           |

## Chua lam (can may that / SQL Server)

| ID  | Task                                | Ly do                                              |
| --- | ----------------------------------- | -------------------------------------------------- |
| F5  | REST API`/api/*` du phong POS       | Can may that + SQL Server de test                  |
| T2  | Test manual toan bo flow            | Can chay app + SQL Server                          |
| T3  | Deploy len IIS/server production    | Can môi trường deploy thực tế                     |

---

## Chi tiet thuc hien

### F4: Export bao cao CSV

`BaoCaoController.cs`:
- Refactor: tach `ApplyFilter()` dung chung cho Index va ExportCsv
- Them `ILogger<BaoCaoController>`
- Them action `ExportCsv(string? tuNgay, string? denNgay)` (GET)
  - Group theo ngay, tinh: so hoa don, gia von, doanh thu, loi nhuan
  - Xuat file CSV voi UTF-8 BOM (de Excel mo dung tieng Viet)
  - Ten file: `bao-cao-doanh-thu_yyyyMMdd_HHmmss.csv`
  - Log khi xuat

`Views/BaoCao/Index.cshtml`:
- Them link "Xuat CSV" canh nut "Loc", giu nguyen filter tuNgay/denNgay

**DoD:**
- [x] Xuat duoc file CSV theo khoang thoi gian
- [x] File mo dung trong Excel (tien Viet khong loi)
- [x] Log xuat CSC

### B7: ILogger con lai

Them `ILogger<T>` cho cac controller chua co:
- SanPhamController, NhanVienController, KhachHangController
- NhaCungCapController, DashboardController, TonKhoController
- VaiTroController, MauSacController, KichThuocController

(Khong them cho TrangChuController vi chi tra ve view khong co logic)

**DoD:**
- [x] Tat ca controller nghiệp vụ co ILogger
- [x] Build 0 warning, 0 error

---

## Definition of Done (Sprint 3)

- [x] `dotnet build QuanLyCuaHangTheThao.sln` - 0 warning, 0 error
- [x] `dotnet test` - 7/7 pass
- [ ] (O lai Sprint sau) Test manual tren may that + deploy
