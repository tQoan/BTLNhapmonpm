using System.ComponentModel.DataAnnotations;

namespace QuanLyCuaHangTheThao.Models;

public class SanPhamViewModel
{
    public int IMaSp { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
    [Display(Name = "Tên sản phẩm")]
    public string STenSp { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
    [Display(Name = "Nhà cung cấp")]
    public int IMaNcc { get; set; }

    [Display(Name = "Xuất xứ")]
    public string? SXuatXu { get; set; }

    [Display(Name = "Giá nhập")]
    public decimal FGiaNhap { get; set; }

    [Display(Name = "Giá bán")]
    public decimal FGiaBan { get; set; }

    [Display(Name = "Mô tả")]
    public string? SMoTa { get; set; }

    [Display(Name = "Ngày hết hạn")]
    public DateTime? DNgayHetHan { get; set; }

    // Loại sản phẩm: quan_ao | giay_dep | dung_cu
    [Display(Name = "Loại sản phẩm")]
    public string? LoaiSanPham { get; set; }

    // Thuộc tính chuyên biệt
    [Display(Name = "Chất liệu")]
    public string? SChatLieu { get; set; }

    [Display(Name = "Kiểu dáng")]
    public string? SKieuDang { get; set; }

    [Display(Name = "Loại đế")]
    public string? SLoaiDe { get; set; }

    [Display(Name = "Công dụng")]
    public string? SCongDung { get; set; }

    // Size & màu được chọn
    public List<int> SelectedSizes { get; set; } = new();
    public List<int> SelectedMaus { get; set; } = new();

    // Hình ảnh sản phẩm
    [Display(Name = "Hình ảnh sản phẩm")]
    public IFormFile? HinhAnh { get; set; }
}
