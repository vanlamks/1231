using System;

namespace PheLieuAPI.Models
{
    public class AdminUserModel
    {
        public Guid Id { get; set; }
        public Guid TaiKhoanId { get; set; }
        public string? HoTen { get; set; }
        public string? GhiChu { get; set; }
        public bool TrangThaiHoatDong { get; set; } = true;

        // 🕒 Thêm 2 dòng này:
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
