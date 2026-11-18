# Phân công công việc - Nhóm 4 người

> Cập nhật: 18/11/2026
> Phiên bản: `v1-3modules` (commit `3a865bf`)
> Phạm vi: 3 chức năng chính - Bán hàng | Kho & Sản phẩm | Báo cáo & Quản trị

---

## 1. Phạm vi 3 chức năng chính

| # | Chức năng | Màn hình | Người phụ trách |
| - | --------- | -------- | --------------- |
| 1 | **Bán hàng** | Bán hàng (POS), Hóa đơn, Khách hàng | N1 |
| 2 | **Kho & Sản phẩm** | Sản phẩm, Nhập hàng, Tồn kho, Nhà cung cấp, Kích thước, Màu sắc | N2 + N3 |
| 3 | **Báo cáo & Quản trị** | Tổng quan, Thống kê, Nhân viên, Vai trò | N4 |

Menu đã gộp từ 5 nhóm xuống đúng 3 nhóm trên (xem `Views/Shared/_Layout.cshtml`).

---

## 2. Bước 0 đã hoàn thành (nền tảng cho cả nhóm)

- [x] `git init` + commit baseline `34cc743` (khôi phục được mọi thứ trước khi cắt).
- [x] Xóa module **Lịch làm việc + Ca làm việc**:
  - `Controllers/LichLamViecController.cs`, `Controllers/CaLamViecController.cs`
  - `Services/LichLamViecTinh.cs`, `Models/LichLamViecViewModel.cs`
  - `Views/LichLamViec/` (2 file), `Views/CaLamViec/` (3 file)
  - `QuanLyCuaHangTheThao.Tests/LichLamViecTinhTests.cs`
- [x] Gộp menu 5 → 3 nhóm, bỏ 3 link "Ca làm việc", "Lịch làm việc", "Lịch của tôi".
- [x] Ẩn "Thống kê" với role không phải Admin/Kế toán; "Quản trị" chỉ Admin thấy (khớp `[Authorize]` trong controller).
- [x] Fix bug: `AccountController.Login` (GET) redirect về `Home/Index` không tồn tại → `TrangChu`.
- [x] `dotnet build`: 0 warning, 0 error. `dotnet test`: 12/12 pass.
- [x] Commit `3a865bf` + tag `v1-3modules`.

**Ghi chú kỹ thuật:** bảng `tblCaLamViec`, `tblLichLamViec` và scaffold model vẫn giữ nguyên trong DB/context (không migrate, không ảnh hưởng build). Script `scripts/ca_lam_viec_upgrade.sql` giữ lại để tạo DB đầy đủ.

---

## 3. Phân công theo người

### N1 - Bán hàng (nhóm trưởng)

**Nhánh:** `n1-banhang`

| File được sửa |
| ------------- |
| `Controllers/BanHangController.cs` |
| `Controllers/HoaDonController.cs` |
| `Controllers/KhachHangController.cs` |
| `Views/BanHang/Index.cshtml` |
| `Views/HoaDon/{Index,ChiTiet,In}.cshtml` |
| `Views/KhachHang/{Index,Create,Edit}.cshtml` |
| `Models/HoaDonViewModel.cs` |

**Việc chính**
1. POS: chọn sản phẩm theo size × màu, hiển thị giá/tồn, giảm giá 0-100%, thanh toán tiền mặt/chuyển khoản, tiền thừa.
2. Chặn bán vượt tồn trước khi lưu (đã có) - kiểm tra lại thông báo lỗi tiếng Việt rõ ràng.
3. Hóa đơn: danh sách + lọc ngày/tìm kiếm, chi tiết, in (A4), xóa (Admin) + hoàn tồn kho.
4. Khách hàng: CRUD + chặn trùng số điện thoại ở Create và Edit.
5. Log các thao tác quan trọng bằng `ILogger`.

**Test tay**
- [ ] Tạo HĐ 2 sản phẩm khác size/màu → tồn kho trừ đúng từng biến thể.
- [ ] Bán số lượng lớn hơn tồn → báo lỗi, không tạo hóa đơn.
- [ ] In hóa đơn hiển thị đúng tên cửa hàng, chi tiết, tổng tiền.
- [ ] Xóa hóa đơn (Admin) → tồn kho được hoàn lại.
- [ ] Thêm khách hàng trùng SĐT → báo lỗi, không lưu.

### N2 - Sản phẩm & danh mục

**Nhánh:** `n2-sanpham`

| File được sửa |
| ------------- |
| `Controllers/SanPhamController.cs` |
| `Controllers/KichThuocController.cs` |
| `Controllers/MauSacController.cs` |
| `Controllers/NhaCungCapController.cs` |
| `Views/SanPham/{Index,_Form,Create,Edit}.cshtml` |
| `Views/KichThuoc/*`, `Views/MauSac/*`, `Views/NhaCungCap/*` |
| `Models/SanPhamViewModel.cs` |

**Việc chính**
1. CRUD sản phẩm: tên, nhà cung cấp, xuất xứ, giá nhập/bán, mô tả, tồn, hạn dùng.
2. Biến thể: chọn size + màu cho sản phẩm (lưu 2 bảng join), phân loại quần áo/giày dép/dụng cụ.
3. Danh mục Kích thước / Màu sắc / Nhà cung cấp: CRUD + chặn trùng tên.
4. Xóa sản phẩm: chưa phát sinh giao dịch thì xóa được, đã bán/đã nhập thì báo lỗi thân thiện (không lộ exception SQL).
5. Log thao tác xóa/sửa giá.

**Test tay**
- [ ] Tạo sản phẩm quần áo có 3 size + 2 màu → lưu đúng.
- [ ] Sửa giá bán, mô tả, loại sản phẩm → form giữ dữ liệu cũ.
- [ ] Xóa sản phẩm chưa phát sinh → OK; sản phẩm đã bán → báo lỗi rõ ràng.
- [ ] Thêm size/màu/NCC trùng tên → báo lỗi.
- [ ] Danh sách sản phẩm: tìm kiếm + phân trang đúng.

### N3 - Nhập hàng & Tồn kho

**Nhánh:** `n3-kho`

| File được sửa |
| ------------- |
| `Controllers/NhapHangController.cs` |
| `Controllers/TonKhoController.cs` |
| `Views/NhapHang/{Index,Create,Edit,Detail}.cshtml` |
| `Views/TonKho/{Index,HanDung}.cshtml` |
| `Models/PhieuNhapViewModel.cs` |

**Việc chính**
1. Phiếu nhập: tạo/sửa/xóa + chi tiết; cộng tồn kho qua trigger `trg_ChiTietPhieuNhap_CapNhatTonKho`.
2. Sửa phiếu nhập: hoàn tồn theo dòng cũ rồi áp dòng mới (đã có) - kiểm tra chênh lệch đúng.
3. Xóa phiếu nhập (Admin) + trừ tồn đã cộng, không để tồn âm.
4. Tồn kho: danh sách + tìm kiếm + phân trang, thống kê sắp hết/hết hàng, màn Hạn dùng.
5. Kiểm chứng 3 trigger trong `database.sql` (chỉ đọc file, muốn sửa thì báo N4).

**Test tay**
- [ ] Nhập 10 → tồn +10.
- [ ] Sửa phiếu 10 → 15 → tồn +5 (không phải +15).
- [ ] Xóa phiếu 15 → tồn -15, không âm.
- [ ] Tồn kho: lọc theo tên, tổng tồn/sắp hết/hết hàng khớp dữ liệu.
- [ ] Màn Hạn dùng hiển thị đúng trạng thái.

### N4 - Nền tảng, Quản trị & Báo cáo

**Nhánh:** `n4-nentang`

| File được sửa |
| ------------- |
| `Controllers/AccountController.cs`, `HomeController.cs` |
| `Controllers/NhanVienController.cs`, `VaiTroController.cs` |
| `Controllers/DashboardController.cs`, `BaoCaoController.cs` |
| `Views/Account/*`, `Views/NhanVien/*`, `Views/VaiTro/*`, `Views/Dashboard/*`, `Views/BaoCao/*` |
| `Views/Shared/*`, `wwwroot/css/admin.css`, `wwwroot/js/admin.js` |
| `Program.cs`, `database.sql`, `.Tests/*`, `docs/*` |

**Việc chính**
1. Đăng nhập / đăng xuất / đổi mật khẩu; phân quyền 3 role (Admin, Bán hàng, Kế toán); trang từ chối truy cập.
2. Nhân viên + tài khoản: CRUD, gán vai trò, không cho xóa nhân viên đã có hóa đơn/phiếu nhập.
3. Vai trò: CRUD, chặn xóa role đang được dùng.
4. Tổng quan: 4 số liệu + chart 7 ngày + top 5 bán chạy (kiểm tra số liệu khớp DB).
5. Thống kê: lọc khoảng ngày, doanh thu/giá vốn/lợi nhuận, export CSV mở đúng tiếng Việt trong Excel.
6. Giữ layout/menu ổn định, chạy `dotnet build` + `dotnet test` cuối cùng trước khi merge.

**Test tay**
- [ ] Đăng nhập 3 role → menu đúng quyền (Bán hàng không thấy Thống kê/Quản trị).
- [ ] Truy cập URL cấm bằng tay → bị chặn / báo AccessDenied.
- [ ] Đổi mật khẩu sai mật khẩu cũ → báo lỗi; đúng → đăng nhập lại được.
- [ ] Dashboard khớp hóa đơn thực tế trong ngày.
- [ ] CSV mở bằng Excel: tiếng Việt không lỗi font, số dòng đúng.

---

## 4. File dùng chung - chỉ N4 được sửa

`Views/Shared/_Layout.cshtml`, `Views/_ViewImports.cshtml`, `wwwroot/css/admin.css`, `wwwroot/js/admin.js`, `Program.cs`, `database.sql`, `QuanLyCuaHangTheThao.csproj`, `.Tests/*`, `docs/*`.

Người khác cần đổi menu/css/db → nhắn N4, không tự sửa để tránh conflict.

---

## 5. Quy trình git

```bash
# Mỗi người tạo nhánh từ main
git checkout main
git pull                # nếu đã có remote GitHub
git checkout -b n1-banhang

# Commit nhỏ, build xanh mới commit
dotnet build QuanLyCuaHangTheThao.sln
git add <file-cua-minh>
git commit -m "feat(pos): chan ban vuot ton kho"
```

**Thứ tự merge:** `n4-nentang` (menu/nền tảng) → `n1-banhang` → `n2-sanpham` → `n3-kho`.

Trước khi merge: `git checkout main` → `git merge --no-ff <nhanh>`; nếu conflict thì người tạo nhánh tự rebase và báo nhóm.

**Quy ước commit:** `<type>(<scope>): <mô tả ngắn>`
- type: `feat`, `fix`, `refactor`, `test`, `docs`, `chore`
- scope: `pos`, `hoa-don`, `khach-hang`, `san-pham`, `kho`, `nhap-hang`, `bao-cao`, `nhan-vien`, `auth`, `ui`

Mỗi người tối thiểu **3 commit** có tên mình để tính điểm đóng góp.

---

## 6. Lịch thực hiện

| Tuần | Nội dung | Kết quả |
| ---- | -------- | ------- |
| Tuần 1 | Bước 0 (xong) + mỗi người đọc vùng code, làm task nhỏ (message lỗi, validation, log) | 4 nhánh có commit đầu |
| Tuần 2 | Hoàn thiện luồng chính từng màn hình | 3 chức năng chạy được trên máy dev |
| Tuần 3 | Test chéo (N1 test kho, N3 test POS...), sửa bug, merge | Bản chạy ổn định trên `main` |
| Tuần 4 | Báo cáo, slide, demo | Nộp bài |

---

## 7. Definition of Done (dùng chung)

- [ ] `dotnet build QuanLyCuaHangTheThao.sln` - 0 warning, 0 error
- [ ] `dotnet test` - 12/12 pass
- [ ] Không còn link/màn hình Lịch làm việc, Ca làm việc trong app
- [ ] 4 luồng chạy được: Đăng nhập → Bán hàng (trừ tồn) → Nhập hàng (cộng tồn) → Báo cáo + CSV
- [ ] Tồn kho luôn khớp sau bán/nhập/sửa/xóa (kiểm tra bằng SQL: `SELECT * FROM v_TonKho`)
- [ ] Code có `ILogger` ở thao tác ghi/xóa quan trọng
- [ ] Thông báo lỗi bằng tiếng Việt, không lộ exception SQL
- [ ] Code được review bởi ít nhất 1 người khác trước khi merge
- [ ] Mỗi người có tên trong ít nhất 3 commit

---

## 8. Ghi chú kỹ thuật

- **Database:** dùng `database.sql` để tạo mới; bảng lịch/ca vẫn tồn tại nhưng app không dùng. Trigger tồn kho là nghiệp vụ quan trọng - không tắt trigger để "cho qua" test.
- **Variant key:** `BanHangController` dùng key `"{size}-{mau}"`, không dùng `size*1000+mau`.
- **Route mặc định:** vẫn là `TrangChu` (landing page). Ai muốn vào app thì đăng nhập, sau đó vào Tổng quan.
- **4 controller trong `Controllers/Api/`** hiện không được view/JS nào gọi (dead code) - giữ nguyên trong đợt này, có thể dọn ở đợt sau nếu cần giảm dòng code trong báo cáo.
