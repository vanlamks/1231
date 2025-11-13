namespace PheLieuAPI.Models
{
    public class ViTriNguoiDungModel
    {
        public Guid Id { get; set; }
        public Guid TaiKhoanId { get; set; }
        public double KinhDo { get; set; }
        public double ViDo { get; set; }
        public DateTime ThoiGianCapNhat { get; set; }
        public string? Email { get; set; }
    }
}
