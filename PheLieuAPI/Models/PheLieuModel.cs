namespace PheLieuAPI.Models
{
    public class PheLieuModel
    {
        public Guid Id { get; set; }
        public string TenPheLieu { get; set; } = string.Empty;
        public string MaLoai { get; set; } = string.Empty;
        public decimal KhoiLuong { get; set; }
        public decimal DonGia { get; set; }
        public string? MoTa { get; set; }
        public string? HinhAnh { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Liên kết
        public string? TenLoai { get; set; }
    }
}
