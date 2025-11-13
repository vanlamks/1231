using System;

namespace PheLieuAPI.Models
{
    public class ThongBaoHeThongModel
    {
        public Guid Id { get; set; }
        public Guid TaiKhoanId { get; set; }
        public string NoiDung { get; set; } = string.Empty;
        public bool DaDoc { get; set; }
        public DateTime NgayGui { get; set; }
    }
}
