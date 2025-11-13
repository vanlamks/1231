using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Helpers;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class OTPResetPasswordService
    {
        private readonly DbHelper _db;
        private readonly string _connectionString;

        public OTPResetPasswordService(DbHelper db, IConfiguration config)
        {
            _db = db;
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // Thêm mã OTP
        public async Task<int> InsertAsync(OTPResetPasswordModel model)
        {
            var parameters = new[]
            {
                new SqlParameter("@TaiKhoanId", model.TaiKhoanId),
                new SqlParameter("@OTPCode", model.OTPCode),
                new SqlParameter("@ThoiGianHetHan", model.ThoiGianHetHan)
            };
            return await _db.ExecuteNonQueryAsync("sp_OTP_ResetPassword_Insert", parameters);
        }

        // Cập nhật mã OTP đã sử dụng
        public async Task<int> MarkAsUsedAsync(Guid taiKhoanId, string otpCode)
        {
            var parameters = new[]
            {
                new SqlParameter("@TaiKhoanId", taiKhoanId),
                new SqlParameter("@OTPCode", otpCode)
            };
            return await _db.ExecuteNonQueryAsync("sp_OTP_ResetPassword_MarkAsUsed", parameters);
        }

        // Lấy OTP theo tài khoản
        public async Task<OTPResetPasswordModel?> GetByTaiKhoanAsync(Guid taiKhoanId)
        {
            var parameters = new[] { new SqlParameter("@TaiKhoanId", taiKhoanId) };
            var table = await _db.ExecuteQueryAsync("sp_OTP_ResetPassword_GetByTaiKhoan", parameters);
            if (table.Rows.Count == 0) return null;

            var row = table.Rows[0];
            return new OTPResetPasswordModel
            {
                Id = row.Field<Guid>("Id"),
                TaiKhoanId = row.Field<Guid>("TaiKhoanId"),
                OTPCode = row.Field<string>("OTPCode"),
                ThoiGianTao = row.Field<DateTime>("ThoiGianTao"),
                ThoiGianHetHan = row.Field<DateTime>("ThoiGianHetHan"),
                DaSuDung = row.Field<bool>("DaSuDung")
            };
        }
    }
}
