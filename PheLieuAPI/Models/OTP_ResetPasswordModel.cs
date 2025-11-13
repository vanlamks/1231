namespace PheLieuAPI.Models
{
    public class OTPResetPasswordModel
    {
        public Guid Id { get; set; }
        public Guid TaiKhoanId { get; set; }
        public string OTPCode { get; set; } = string.Empty; // Mã OTP
        public DateTime ThoiGianTao { get; set; } // Thời gian tạo OTP
        public DateTime ThoiGianHetHan { get; set; } // Thời gian hết hạn OTP
        public bool DaSuDung { get; set; } // Trạng thái đã sử dụng hay chưa
    }
}
