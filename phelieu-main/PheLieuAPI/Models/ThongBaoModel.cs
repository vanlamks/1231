namespace PheLieuAPI.Models
{
    public class ThongBaoModel
    {
        public Guid Id { get; set; }
        public Guid DoanhNghiepId { get; set; }
        public string Loai { get; set; } = string.Empty;
        public Guid? DonBanId { get; set; }
        public Guid? DonMuaId { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public bool DaXem { get; set; }
        public DateTime CreatedAt { get; set; }

        // Dữ liệu bổ sung để hiển thị dễ hơn
        public string? TenPheLieu { get; set; }
        public string? TenNguoiDang { get; set; }
    }
}
