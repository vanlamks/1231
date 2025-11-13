using System;

namespace PheLieuAPI.Models
{
    public class KhachHangModel
    {
        public Guid Id { get; set; }
        public Guid TaiKhoanId { get; set; }
        public string? Email { get; set; }
        public string? SoDienThoai { get; set; }
        public string HoTen { get; set; } = "";
        public string? DiaChiText { get; set; }
        public string? GhiChu { get; set; }
        public bool TrangThaiHoatDong { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
