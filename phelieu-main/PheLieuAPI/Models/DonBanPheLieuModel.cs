using System;

namespace PheLieuAPI.Models
{
    public class DonBanPheLieuModel
    {
        public Guid Id { get; set; }
        public Guid? KhachHangId { get; set; }
        public Guid? DoanhNghiepId { get; set; }
        public string TenPheLieu { get; set; } = string.Empty;
        public decimal KhoiLuong { get; set; }
        public decimal DonGia { get; set; }
        public string MoTa { get; set; } = string.Empty;
        public string TrangThai { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string NguoiDang { get; set; } = string.Empty;
        public string LoaiNguoiDang { get; set; } = string.Empty;
    }
}
