namespace PheLieuAPI.Models
{
    public class LichHenModel
    {
        public Guid Id { get; set; }
        public Guid KhachHangId { get; set; }
        public string? TenKhachHang { get; set; }
        public string? DiaChi { get; set; }
        public DateTime ThoiGianHen { get; set; }
        public string TrangThaiCode { get; set; } = string.Empty;
        public string? TenTrangThai { get; set; }
        public string? GhiChu { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
