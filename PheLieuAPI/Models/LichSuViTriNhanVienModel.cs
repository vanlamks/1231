namespace PheLieuAPI.Models
{
    public class LichSuViTriNhanVienModel
    {
        public Guid Id { get; set; }
        public Guid NhanVienId { get; set; }
        public double KinhDo { get; set; }
        public double ViDo { get; set; }
        public DateTime ThoiGian { get; set; }
        public string? TenNhanVien { get; set; }
    }
}
