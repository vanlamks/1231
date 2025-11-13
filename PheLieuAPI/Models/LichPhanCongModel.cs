namespace PheLieuAPI.Models
{
    public class LichPhanCongModel
    {
        public Guid Id { get; set; }
        public Guid DoanhNghiepId { get; set; }
        public Guid NhanVienId { get; set; }
        public string CongViec { get; set; } = "";
        public string DiaDiem { get; set; } = "";
        public float KinhDo { get; set; }
        public float ViDo { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public string TrangThai { get; set; } = "";
        public DateTime HanPhanHoi { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TenNhanVien { get; set; } = "";
    }
}
