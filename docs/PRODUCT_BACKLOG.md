# Product Backlog - Quan Ly Cua Hang The Thao

> Cap nhat lan cuoi: 08/09/2026
> Tong story points: 46 SP
> Thoi gian du kien: 2-3 tuan (3 Sprint)

---

## A. Don dep & Nen tang

| ID  | Task                                                               | SP | Priority | Sprint | Status     |
| --- | ------------------------------------------------------------------ | -- | -------- | ------ | ---------- |
| B1  | Xoa `frontend/` template dead code (BoomerangVideoBg, dist/, node_modules) | 1 | Cao      | S1     | Todo       |
| B2  | Xoa `HomeController` dead scaffold (Home/Index, Privacy)          | 1 | Thap     | S2     | Todo       |
| B3  | Tao `database.sql` (schema + triggers + views + seed data)        | 3 | Cao      | S1     | Todo       |
| B4  | Fix `Program.cs` lazy upgrade password (khong load toan bo accounts) | 2 | Trung binh | S2  | Todo       |
| B5  | Fix variant key collision trong `BanHangController` (size*1000+mau) | 2 | Trung binh | S1  | Todo       |
| B6  | Xoa `_LandingLayout.cshtml` dead reference (window.foldcraft)     | 1 | Thap     | S2     | Todo       |
| B7  | Them ILogger cho tat ca Controllers                               | 3 | Trung binh | S1-3 | Todo       |

## B. Backend - Cai thien tinh nang

| ID  | Task                                                               | SP | Priority | Sprint | Status     |
| --- | ------------------------------------------------------------------ | -- | -------- | ------ | ---------- |
| F1  | Xoa hoa don (HoaDon Delete) - Admin only                         | 3 | Trung binh | S2   | Todo       |
| F2  | Xoa phieu nhap (NhapHang Delete) + undo ton kho                  | 3 | Trung binh | S2   | Todo       |
| F3  | Validation duplicate SĐT Khach hang                              | 2 | Trung binh | S1   | Todo       |
| F4  | Export bao cao ra Excel/CSV (dung EPPlus/ClosedXML)              | 5 | Thap     | S3     | Todo       |
| F5  | REST API endpoints `/api/*` (du phong cho POS React tuong lai)   | 8 | Thap     | S3     | Todo       |

## C. Frontend

| ID  | Task                                                               | SP | Priority | Sprint | Status     |
| --- | ------------------------------------------------------------------ | -- | -------- | ------ | ---------- |
| C1  | Xoa `frontend/` directory hoan toan                              | 1 | Cao      | S1     | Todo       |
| C2  | (Tu chon) Tao React POS ket noi API - thuc hien sau Sprint 3     | 8 | Thap     | Sau S3 | Todo       |

## D. Testing & Deploy

| ID  | Task                                                               | SP | Priority | Sprint | Status     |
| --- | ------------------------------------------------------------------ | -- | -------- | ------ | ---------- |
| T1  | Unit test cho PasswordHasher + BanHang stock validation           | 3 | Trung binh | S2   | Todo       |
| T2  | Test manual toan bo flow: Login > Ban hang > Nhap hang > Bao cao | 2 | Cao      | S3     | Todo       |
| T3  | Deploy len IIS/server + cau hinh connection string production    | 3 | Thap     | S3     | Todo       |

---

## Phan cong vai tro Scrum

| Vai tro        | Nguoi   | Trach nhiem                                    |
| -------------- | ------- | ---------------------------------------------- |
| Product Owner  | Nhom truong | Quyet dinh uu tien, dat yeu cau           |
| Scrum Master   | (chon 1) | Danh dot qua trinh, to chuc hop, goi block   |
| Developer 1    | (chon)  | Backend: API, fix bug, database script         |
| Developer 2    | (chon)  | Frontend: Razor views, testing, deploy         |

## Scrum Events

- **Daily Standup**: Moi sang 9:00-9:15 (Thu 2-6)
- **Sprint Review**: Thu 6 chieu 15:30-16:00
- **Sprint Retrospective**: Thu 6 chieu 16:00-16:30
- **Sprint Planning**: Thu 6 chieu 16:30-17:00 (cuoi Sprint)

## Definition of Done (DoD)

- [ ] Code build thanh cong (dotnet build khong loi)
- [ ] Khong co loi runtime khi chay chuc nang
- [ ] Da test tren local may dev
- [ ] Code duoc review boi it nhat 1 nguoi khac
- [ ] Commit message ro rang, mo ta cong viec da lam
