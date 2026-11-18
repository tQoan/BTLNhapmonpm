# -*- coding: utf-8 -*-
"""Tao file BaoCaoLan1.docx"""
from docx import Document
from docx.shared import Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH

doc = Document()

# Font
st = doc.styles['Normal']
st.font.name = 'Times New Roman'; st.font.size = Pt(13)
st.paragraph_format.line_spacing = 1.5; st.paragraph_format.space_after = Pt(6)
for lv, sz in [('Heading 1', 16), ('Heading 2', 14)]:
    s = doc.styles[lv]; s.font.name = 'Times New Roman'; s.font.size = Pt(sz)
    s.font.bold = True; s.font.color.rgb = RGBColor(0,0,0)
    s.paragraph_format.space_before = Pt(12); s.paragraph_format.space_after = Pt(6)

def P(t, b=False, a=WD_ALIGN_PARAGRAPH.JUSTIFY):
    p=doc.add_paragraph(); p.alignment=a; r=p.add_run(t); r.font.name='Times New Roman'; r.font.size=Pt(13); r.bold=b; return p
def BN(bt, nt):
    p=doc.add_paragraph(); p.alignment=WD_ALIGN_PARAGRAPH.JUSTIFY
    r1=p.add_run(bt); r1.bold=True; r1.font.name='Times New Roman'; r1.font.size=Pt(13)
    r2=p.add_run(nt); r2.font.name='Times New Roman'; r2.font.size=Pt(13); return p
def B(t):
    p=doc.add_paragraph(style='List Bullet'); p.clear(); r=p.add_run(t); r.font.name='Times New Roman'; r.font.size=Pt(13); return p
def NP(bp, t):
    p=doc.add_paragraph(); p.alignment=WD_ALIGN_PARAGRAPH.JUSTIFY
    r1=p.add_run(bp); r1.bold=True; r1.font.name='Times New Roman'; r1.font.size=Pt(13)
    r2=p.add_run(t); r2.font.name='Times New Roman'; r2.font.size=Pt(13); return p

# ===== TRANG BIA =====
for _ in range(6): doc.add_paragraph()
P('ĐỀ ÁN / BÀI TẬP LỚN', b=True, a=WD_ALIGN_PARAGRAPH.CENTER)
P('ĐỀ TÀI:', b=True, a=WD_ALIGN_PARAGRAPH.CENTER)
P('XÂY DỰNG HỆ THỐNG QUẢN LÝ CỬA HÀNG BÁN ĐỒ THỂ THAO', b=True, a=WD_ALIGN_PARAGRAPH.CENTER)
P('(Web Application - ASP.NET Core MVC & SQL Server)', a=WD_ALIGN_PARAGRAPH.CENTER)
doc.add_paragraph()
P('BÁO CÁO QUY TRÌNH PHÂN TÍCH THIẾT KẾ VÀ TRIỂN KHAI', b=True, a=WD_ALIGN_PARAGRAPH.CENTER)
P('Lần 1: Tổng quan đề tài, Giới thiệu bài toán, Mục tiêu, Hệ thống liên quan', a=WD_ALIGN_PARAGRAPH.CENTER)
for _ in range(4): doc.add_paragraph()
P('Ngày lập: 16/09/2026', a=WD_ALIGN_PARAGRAPH.CENTER)
doc.add_page_break()

# ================================================================
# 1.1 TONG QUAN VE DE TAI
# ================================================================
doc.add_heading('1.1. Tổng quan về đề tài', level=1)

BN('Tên đề tài: ', 'Xây dựng hệ thống quản lý cửa hàng bán đồ thể thao')

P('Trong bối cảnh kinh tế thị trường ngày càng cạnh tranh, các cửa hàng bán lẻ đồ thể thao '
  'đối mặt với nhiều thách thức trong việc quản lý hoạt động kinh doanh. Quy trình quản lý '
  'thủ công bằng sổ sách hoặc bằng các bảng Excel dẫn đến nhiều hạn chế: không thể theo dõi '
  'tồn kho chính xác theo thời gian thực, khó tổng hợp báo cáo doanh thu, dễ xảy ra sai sót '
  'trong giao dịch, và khó kiểm soát phân quyền truy cập dữ liệu.')

P('Đề tài "Xây dựng hệ thống quản lý cửa hàng bán đồ thể thao" được thực hiện với mục đích '
  'hỗ trợ các cửa hàng bán lẻ đồ thể thao trong việc tự động hóa các quy trình nghiệp vụ '
  'chính: quản lý sản phẩm, quản lý tồn kho, bán hàng, nhập hàng, quản lý khách hàng, nhân '
  'viên, nhà cung cấp, và báo cáo doanh thu. Hệ thống được xây dựng dưới dạng web application '
  'sử dụng công nghệ ASP.NET Core MVC với cơ sở dữ liệu SQL Server.')

BN('Đối tượng áp dụng: ',
    'Hệ thống được thiết kế dành cho các cửa hàng bán lẻ đồ thể thao, gồm ba nhóm mặt hàng '
    'chính: quần áo thể thao, giày dép thể thao, và dụng cụ thể thao. Hệ thống phù hợp với '
    'cửa hàng đơn chi nhánh hoặc nhiều chi nhánh nhỏ, với quy mô từ 3 đến 10 nhân viên.')

BN('Phạm vi hệ thống: ', 'Hệ thống bao gồm các chức năng chính sau:')

B('Quản lý sản phẩm: thêm, sửa, xóa, tìm kiếm sản phẩm theo nhóm (quần áo, giày dép, dụng cụ), quản lý kích thước (size) và màu sắc.')
B('Quản lý tồn kho: theo dõi tồn kho chi tiết theo từng sản phẩm - kích thước - màu sắc, cảnh báo hết hàng và sản phẩm sắp hết hạn.')
B('Bán hàng (POS): lập hóa đơn bán hàng động, tích hợp thanh toán đa phương thức (tiền mặt, chuyển khoản với mã QR VietQR).')
B('Nhập hàng: lập phiếu nhập từ nhà cung cấp, cập nhật tồn kho tự động qua trigger trong cơ sở dữ liệu.')
B('Quản lý khách hàng, nhà cung cấp, nhân viên, vai trò.')
B('Báo cáo doanh thu: thống kê doanh thu, lợi nhuận, sản phẩm bán chạy, xuất file CSV.')
B('Dashboard tổng quan: hiển thị tổng quan doanh thu, tồn kho, số lượng hóa đơn.')
B('Đăng nhập / phân quyền theo vai trò (Cookie Authentication + Role-based Authorization).')

# ================================================================
# 1.2 GIOI THIEU BAI TOAN
# ================================================================
doc.add_heading('1.2. Giới thiệu bài toán', level=1)

doc.add_heading('1.2.1. Bài toán cốt lõi', level=2)

P('Bài toán cốt lõi của đề tài là xây dựng một hệ thống quản lý tập trung, giúp các cửa '
  'hàng bán lẻ đồ thể thao quản lý toàn bộ quy trình kinh doanh từ nhập hàng đến bán hàng '
  'và báo cáo. Hệ thống cần đảm bảo dữ liệu được lưu trữ an toàn, nhất quán, và có thể '
  'truy xuất bất kỳ lúc nào.')

P('Cụ thể, hệ thống cần giải quyết các bài toán:')

B('Làm thế nào để theo dõi tồn kho chính xác theo từng sản phẩm, kích thước và màu sắc, thay vì chỉ theo dõi tổng thể như các hệ thống thông thường.')
B('Làm thế nào để tự động cập nhật tồn kho khi có giao dịch bán hàng hoặc nhập hàng, tránh sai sót do thao tác thủ công.')
B('Làm thế nào để phân quyền truy cập phù hợp với từng vai trò nhân viên (quản trị viên, bán hàng, kế toán, quản lý kho).')
B('Làm thế nào để cung cấp báo cáo doanh thu, lợi nhuận theo thời gian thực giúp chủ cửa hàng ra quyết định kinh doanh.')

doc.add_heading('1.2.2. Hiện trạng quản lý', level=2)

P('Hiện tại, nhiều cửa hàng bán lẻ đồ thể thao vẫn sử dụng phương pháp quản lý thủ công, '
  'bao gồm:')

B('Sổ sách ghi chép: ghi chép số liệu bán hàng, nhập hàng bằng tay, dễ mất hoặc hư hỏng.')
B('Bảng Excel: theo dõi tồn kho và doanh thu, nhưng khó tính toán chính xác khi số lượng sản phẩm lớn, nhiều kích thước và màu sắc.')
B('Không có hệ thống báo cáo: chủ cửa hàng khó tổng hợp doanh thu theo ngày, tháng, hoặc phân tích sản phẩm bán chạy.')
B('Không phân quyền: tất cả nhân viên đều có quyền truy cập toàn bộ dữ liệu, dễ xảy ra lộ dữ liệu hoặc sửa đổi giao dịch.')
B('Tồn kho không chính xác: không thể theo dõi chi tiết tồn kho theo từng kích thước và màu sắc, dẫn đến thiếu hoặc dư hàng.')

doc.add_heading('1.2.3. Yêu cầu chức năng chính', level=2)

P('Hệ thống cần đảm bảo các chức năng sau:')

NP('1. Đăng nhập / Phân quyền: ',
    'Hệ thống sử dụng Cookie Authentication với role-based authorization. Các vai trò bao gồm: '
    'Admin (toàn quyền), Bán hàng (lập hóa đơn, xem sản phẩm), Kế toán (xem báo cáo), '
    'Quản lý kho (quản lý tồn kho, nhập hàng). Mật khẩu được mã hóa bằng PBKDF2 hash, '
    'tự động nâng cấp từ plaintext khi khởi động ứng dụng.')

NP('2. Quản lý sản phẩm: ',
    'Thêm, sửa, xóa, tìm kiếm sản phẩm. Sản phẩm được phân loại thành 3 nhóm: Quần áo (chất liệu, '
    'kiểu dáng), Giày dép (chất liệu, loại đế), Dụng cụ (chất liệu, công dụng). Mỗi sản phẩm '
    'có nhiều kích thước (size: S, M, L, XL, XXL) và màu sắc (Đen, Trắng, Đỏ, Xanh dương, Xanh lá).')

NP('3. Bán hàng (POS): ',
    'Lập hóa đơn bán hàng động với nhiều dòng sản phẩm. Chọn sản phẩm - kích thước - màu sắc - '
    'số lượng. Tự động tính tổng tiền, áp dụng giảm giá (%). Hỗ trợ thanh toán đa phương thức: '
    'tiền mặt, chuyển khoản (hiển thị mã QR VietQR với mã giao dịch), thẻ. In hóa đơn theo mẫu.')

NP('4. Nhập hàng: ',
    'Lập phiếu nhập từ nhà cung cấp, chọn sản phẩm - kích thước - màu sắc - số lượng - đơn giá nhập. '
    'Tồn kho tự động tăng khi lưu phiếu nhập (thông qua trigger trg_ChiTietPhieuNhap_CapNhatTonKho).')

NP('5. Quản lý tồn kho: ',
    'Theo dõi tồn kho chi tiết theo từng sản phẩm - kích thước - màu sắc. Trigger '
    'trg_ChiTietHoaDon_KiemTraTonVaCapNhat kiểm tra tồn kho trước khi bán hàng. Trigger '
    'trg_TonKhoChiTiet_CapNhatTongSanPham cập nhật tổng số lượng tồn trên bảng tblSanPham. '
    'Cảnh báo sản phẩm sắp hết hàng (xem qua view v_TonKho).')

NP('6. Báo cáo doanh thu: ',
    'Thống kê doanh thu theo ngày, tính giá vốn, lợi nhuận gộp. View v_BaoCaoDoanhThu tổng hợp '
    'doanh thu trước giảm giá, doanh thu thực thu, giá vốn, lợi nhuận. Xuất báo cáo ra file CSV '
    'với UTF-8 BOM để mở đúng trong Excel.')

NP('7. Quản lý khách hàng, nhà cung cấp, nhân viên: ',
    'CRUD đầy đủ cho khách hàng (tên, SĐT, địa chỉ - validate trùng SĐT), nhà cung cấp (tên, '
    'địa chỉ, SĐT), nhân viên (tên, giới tính, SĐT, vai trò).')

doc.add_heading('1.2.4. Yêu cầu phi chức năng', level=2)

B('Bảo mật mật khẩu: sử dụng PBKDF2 hash với 100,000 iterations. Mật khẩu plaintext tự động nâng cấp khi khởi động ứng dụng.')
B('Phân quyền truy cập: chỉ Admin được xóa hóa đơn / phiếu nhập. Nhân viên bán hàng không được truy cập báo cáo. Nhân viên không phải Admin không được sửa dữ liệu tài chính.')
B('Toàn vẹn dữ liệu: trigger trong SQL Server tự động cập nhật tồn kho khi nhập/xuất hàng, đảm bảo dữ liệu luôn nhất quán giữa ứng dụng và cơ sở dữ liệu.')
B('Giao diện responsive: giao diện thân thiện với người dùng, hỗ trợ trên nhiều kích cỡ màn hình, phong cách hiện đại với dark mode.')
B('Ghi log: tất cả controller nghiệp vụ có ILogger để ghi log lỗi và thông tin quan trọng (tạo hóa đơn, đăng nhập, xóa dữ liệu...).')
B('Xử lý lỗi: trang Error.cshtml xử lý ngoại lệ toàn cục. Kiểm tra validation trước khi lưu dữ liệu.')

# ================================================================
# 1.3 MUC TIEU DE TAI
# ================================================================
doc.add_heading('1.3. Mục tiêu của đề tài', level=1)

doc.add_heading('1.3.1. Mục tiêu chung', level=2)

P('Xây dựng một hệ thống web application đầy đủ chức năng, hỗ trợ các cửa hàng bán lẻ '
  'đồ thể thao quản lý hoạt động kinh doanh hiệu quả, giảm thiểu thao tác thủ công, '
  'tăng độ chính xác của dữ liệu, và cung cấp các báo cáo thống kê hấp dẫn để hỗ trợ '
  'quá trình ra quyết định.')

doc.add_heading('1.3.2. Mục tiêu cụ thể', level=2)

B('Xây dựng hệ thống trên nền tảng ASP.NET Core MVC với .NET 8, sử dụng Entity Framework Core 8 (Database-First) kết nối với SQL Server.')
B('Thiết kế cơ sở dữ liệu quan hệ với 16 bảng chính, 4 view báo cáo, 3 trigger tự động cập nhật tồn kho, đảm bảo toàn vẹn dữ liệu ở cấp database.')
B('Triển khai hệ thống đăng nhập / phân quyền với Cookie Authentication, phân quyền theo vai trò (Admin, Bán hàng, Kế toán, Quản lý kho), mật khẩu mã hóa PBKDF2.')
B('Xây dựng giao diện Razor Views thân thiện người dùng, responsive, với dark mode, sidebar navigation, animation mượt, và trang chủ có video hero.')
B('Triển khai module bán hàng (POS) với hỗ trợ nhiều dòng sản phẩm, chọn size/mau, tính tổng tiền, giảm giá, thanh toán đa phương thức (tiền mặt, chuyển khoản với QR VietQR).')
B('Triển khai module quản lý tồn kho chi tiết theo từng sản phẩm - kích thước - màu sắc, với cảnh báo hết hàng và hạn dùng.')
B('Xây dựng hệ thống báo cáo doanh thu với view v_BaoCaoDoanhThu, hỗ trợ xuất file CSV (UTF-8 BOM) phục vụ in ấn và phân tích.')
B('Triển khai unit test với xUnit (7 test cases cho PasswordHasher), đảm bảo chất lượng mã nguồn.')
B('Áp dụng Scrum trong quản lý dự án: Product Backlog (46 story points), 3 Sprint (2-3 tuần), Daily Standup, Sprint Review, Retrospective.')

doc.add_heading('1.3.3. Công nghệ sử dụng', level=2)

table = doc.add_table(rows=9, cols=2)
table.style = 'Light Grid Accent 1'
headers = [('Thành phần', 'Công nghệ / Phiên bản')]
data = [
    ('Nền tảng', 'ASP.NET Core MVC (.NET 8.0)'),
    ('Ngôn ngữ', 'C#'),
    ('ORM', 'Entity Framework Core 8.0.11 (Database-First)'),
    ('Cơ sở dữ liệu', 'SQL Server (SQLEXPRESS)'),
    ('Giao diện', 'Razor Views + HTML/CSS/JavaScript'),
    ('Biểu đồ', 'Chart.js'),
    ('Xác thực', 'Cookie Authentication + Role-based Authorization'),
    ('Test', 'xUnit'),
]
for i, (c1, c2) in enumerate([(headers[0])] + data):
    table.cell(i, 0).text = c1
    table.cell(i, 1).text = c2
    if i == 0:
        for cell in table.rows[i].cells:
            for p in cell.paragraphs:
                for r in p.runs:
                    r.bold = True

# ================================================================
# 1.4 HE THONG LIEN QUAN
# ================================================================
doc.add_heading('1.4. Các đề tài / hệ thống liên quan', level=1)

P('Để tham khảo và học hỏi kinh nghiệm, nhóm đã nghiên cứu một số hệ thống quản lý '
  'bán hàng và quản lý kho đã có trên thị trường, cũng như một số đề tài tương tự trong '
  'lĩnh vực học tập.')

doc.add_heading('1.4.1. Hệ thống quản lý bán hàng Sapo POS', level=2)

P('Sapo POS là một trong những hệ thống quản lý bán hàng được sử dụng rộng rãi tại Việt Nam. '
  'Hệ thống hỗ trợ quản lý sản phẩm, kho hàng, bán hàng, khách hàng, và báo cáo doanh thu. '
  'Sapo cung cấp giao diện đơn giản, dễ sử dụng, với tính năng đồng bộ dữ liệu tức thời '
  '(realtime) giữa các chi nhánh.')

BN('Điểm tương đồng: ',
    'Quản lý sản phẩm theo kích thước và màu sắc, quản lý tồn kho, bán hàng POS, báo cáo doanh thu.')

BN('Điểm khác biệt: ',
    'Sapo là hệ thống SaaS (dịch vụ đăng ký), phí hàng tháng. Hệ thống của nhóm là web application '
    'mã nguồn mở, tự triển khai, không phải trả phí dịch vụ.')

doc.add_heading('1.4.2. Hệ thống quản lý bán hàng KiotViet', level=2)

P('KiotViet là hệ thống quản lý bán hàng và quản lý kho phổ biến tại Việt Nam, '
  'được nhiều cửa hàng, siêu thị, và chuỗi bán lẻ sử dụng. Hệ thống hỗ trợ quản lý '
  'sản phẩm, kho hàng, bán hàng, nhập hàng, khách hàng thân thiết (loyalty), và báo cáo '
  'doanh thu chi tiết.')

BN('Điểm tương đồng: ',
    'Quản lý sản phẩm phân loại rõ ràng, quản lý tồn kho chi tiết, bán hàng nhiều phương thức '
    'thanh toán, báo cáo doanh thu theo nhiều chiều.')

BN('Điểm khác biệt: ',
    'KiotViet hỗ trợ tích hợp với nhiều cổng thanh toán (VNPay, MoMo, ZaloPay), hệ thống tự động '
    'gửi hóa đơn điện tử. Hệ thống của nhóm đơn giản hơn nhưng tập trung vào nghiệp vụ thể thao.')

doc.add_heading('1.4.3. Hệ thống quản lý bán hàng Nhanh.vn', level=2)

P('Nhanh.vn là nền tảng thương mại điện tử hỗ trợ quản lý bán hàng online và offline. '
  'Hệ thống cung cấp tính năng quản lý sản phẩm, kho hàng, đơn hàng, giao hàng, '
  'và tích hợp với các sàn thương mại điện tử (Shopee, Lazada, Tiki).')

BN('Điểm tương đồng: ',
    'Quản lý sản phẩm với nhiều biến thể (size, màu), quản lý kho, báo cáo doanh thu.')

BN('Điểm khác biệt: ',
    'Nhanh.vn tập trung vào kết nối với sàn thương mại điện tử, hỗ trợ giao hàng '
    '(tích hợp giao hàng nhanh). Hệ thống của nhóm tập trung vào bán hàng tại cửa hàng (POS).')

doc.add_heading('1.4.4. Hệ thống quản lý kho ERPNext', level=2)

P('ERPNext là hệ thống ERP (Enterprise Resource Planning) mã nguồn mở, được xây dựng '
  'trên nền tảng Python/Frappe. Hệ thống hỗ trợ quản lý kho hàng, bán hàng, nhập hàng, '
  'sản xuất, tài chính, và nhiều chức năng khác.')

BN('Điểm tương đồng: ',
    'Quản lý tồn kho chi tiết, phiếu nhập/xuất, báo cáo tài chính, phân quyền người dùng.')

BN('Điểm khác biệt: ',
    'ERPNext là hệ thống ERP đầy đủ, quá phức tạp cho cửa hàng nhỏ. Sử dụng Python thay vì C#. '
    'Hệ thống của nhóm tập trung vào nghiệp vụ bán lẻ đồ thể thao, đơn giản và dễ sử dụng hơn.')

doc.add_heading('1.4.5. Hệ thống quản lý phòng tập GymMaster', level=2)

P('GymMaster là hệ thống quản lý phòng tập thể dục / gym, hỗ trợ quản lý hội viên, '
  'lịch tập, thanh toán phí tập, và báo cáo hoạt động. Hệ thống phổ biến tại '
  'nhiều quốc gia, dành cho các phòng tập từ nhỏ đến lớn.')

BN('Điểm tương đồng: ',
    'Quản lý hội viên / khách hàng, thanh toán phí theo thời gian, báo cáo hoạt động.')

BN('Điểm khác biệt: ',
    'GymMaster tập trung vào quản lý phòng tập (lịch tập, hội viên, PT). '
    'Hệ thống của nhóm tập trung vào quản lý cửa hàng bán lẻ (sản phẩm, tồn kho, bán hàng).')

doc.add_heading('1.4.6. Đề tài tương tự trong lĩnh vực học tập', level=2)

P('Trong lĩnh vực học tập, có nhiều đề tài tương tự đã được thực hiện tại các trường đại học, '
  'bao gồm:')

B('Hệ thống quản lý cửa hàng quần áo thời trang: sử dụng PHP/MySQL hoặc Java/MySQL, tập trung vào quản lý sản phẩm thời trang.')
B('Hệ thống quản lý bán hàng tại cửa hàng: sử dụng ASP.NET hoặc Java Spring, hỗ trợ POS và quản lý kho.')
B('Ứng dụng quản lý phòng gym: sử dụng Node.js hoặc Python Django, quản lý hội viên và lịch tập.')
B('Hệ thống quản lý kho hàng đa chi nhánh: sử dụng .NET Core hoặc Java, hỗ trợ đồng bộ dữ liệu giữa các chi nhánh.')

P('Đề tài này được xây dựng dựa trên kinh nghiệm từ các hệ thống trên, kết hợp với yêu cầu '
  'cụ thể của ngành bán lẻ đồ thể thao, nhằm tạo ra một giải pháp phù hợp và hiệu quả.')

# ===== LUU FILE =====
doc.save('BaoCaoLan1.docx')
print('Da tao thanh cong: BaoCaoLan1.docx')
