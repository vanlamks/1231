namespace PheLieuAPI.Models
{
    
    public class DoanhNghiepModel
    {
        public Guid Id { get; set; }
        public Guid TaiKhoanId { get; set; }
        public string TenDoanhNghiep { get; set; } = string.Empty;
        public string? MaSoThue { get; set; }
        public string? DiaChiText { get; set; }
        public string? Website { get; set; }
        public string? MoTa { get; set; }
        public bool Verified { get; set; } = false;
        public bool TrangThaiHoatDong { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
