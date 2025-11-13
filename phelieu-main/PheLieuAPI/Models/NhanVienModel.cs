namespace PheLieuAPI.Models
{
    public class NhanVienModel
    {
        public Guid Id { get; set; }
        public Guid TaiKhoanId { get; set; }
        public Guid? DoanhNghiepId { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public bool TrangThaiSanSang { get; set; } = true;
        public bool TrangThaiHoatDong { get; set; } = true;
        public string? TenDoanhNghiep { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
