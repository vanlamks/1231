using System;

namespace PheLieuAPI.Models
{
    public class TaiKhoanModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string VaiTro { get; set; } = string.Empty; // ADMIN, KHACH_HANG, DOANH_NGHIEP, NHAN_VIEN
        public bool TrangThaiHoatDong { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // ✅ Liên kết dữ liệu vai trò
        public KhachHangModel? ThongTinKhachHang { get; set; }
        public DoanhNghiepModel? ThongTinDoanhNghiep { get; set; }
        public NhanVienModel? ThongTinNhanVien { get; set; }
        public AdminUserModel? ThongTinAdmin { get; set; }
    }

    public class DangNhapModel
    {
        public string TaiKhoan { get; set; } = string.Empty;
        public string MatKhau { get; set; } = string.Empty;
    }

   public class DangKyModel
{
    public string Email { get; set; } = string.Empty;
    public string MatKhau { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;

    // 🧩 Thông tin riêng cho từng vai trò
    public string? HoTen { get; set; }             // KH, NV, Admin
    public string? DiaChiText { get; set; }        // KH
    public string? GhiChu { get; set; }            // KH hoặc Admin

    public string? TenDoanhNghiep { get; set; }    // DN
    public string? MaSoThue { get; set; }          // DN
    public string? Website { get; set; }           // DN
    public string? MoTa { get; set; }              // DN

    private string _vaiTro = "KHACH_HANG";
    public string VaiTro
    {
        get => _vaiTro;
        set
        {
            var allowed = new[] { "ADMIN", "KHACH_HANG", "DOANH_NGHIEP", "NHAN_VIEN" };
            if (!allowed.Contains(value.ToUpper()))
                throw new ArgumentException("Vai trò không hợp lệ!");
            _vaiTro = value.ToUpper();
        }
    }
}
}
