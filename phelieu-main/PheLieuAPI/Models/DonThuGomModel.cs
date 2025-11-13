namespace PheLieuAPI.Models
{
    public class DonThuGomModel
    {
        public Guid Id { get; set; }
        public Guid LichHenId { get; set; }
        public Guid NhanVienId { get; set; }
        public Guid DoanhNghiepId { get; set; }
        public string TrangThaiCode { get; set; } = string.Empty;
        public decimal TongTien { get; set; }
        public string PhuongThucTT { get; set; } = string.Empty;
        public string? GhiChu { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Thông tin mở rộng
        public string? TenNhanVien { get; set; }
        public string? TenDoanhNghiep { get; set; }
        public string? DiaChi { get; set; }
        public DateTime? ThoiGianHen { get; set; }
    }
}
