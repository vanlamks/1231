using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Helpers;
using PheLieuAPI.Models;
using System.Net.Mail;

namespace PheLieuAPI.Services
{
    public class TaiKhoanService
    {
        private readonly DbHelper _db;
        private readonly string _connectionString;
        private readonly string _smtpEmail;
        private readonly string _smtpPass;

        public TaiKhoanService(DbHelper db, IConfiguration config)
        {
            _db = db;
            _connectionString = config.GetConnectionString("DefaultConnection");
            _smtpEmail = config["SmtpSettings:Email"] ?? "your_gmail@gmail.com";
            _smtpPass = config["SmtpSettings:Password"] ?? "app_password";
        }

        // 🧠 Lấy toàn bộ tài khoản
        public async Task<List<TaiKhoanModel>> GetAllAsync()
        {
            var table = await _db.ExecuteQueryAsync("sp_TaiKhoan_GetAll");
            return table.AsEnumerable().Select(row => new TaiKhoanModel
            {
                Id = row.Field<Guid>("Id"),
                Email = row.Field<string>("Email"),
                MatKhau = row.Field<string>("MatKhau"),
                SoDienThoai = row.Field<string>("SoDienThoai"),
                VaiTro = row.Field<string>("VaiTro"),
                TrangThaiHoatDong = row.Field<bool>("TrangThaiHoatDong"),
                CreatedAt = row.Field<DateTime>("CreatedAt"),
                UpdatedAt = row.Field<DateTime>("UpdatedAt")
            }).ToList();
        }

        // 🔍 Lấy theo ID
        public async Task<TaiKhoanModel?> GetByIdAsync(Guid id)
        {
            var parameters = new[] { new SqlParameter("@Id", id) };
            var table = await _db.ExecuteQueryAsync("sp_TaiKhoan_GetById", parameters);
            if (table.Rows.Count == 0) return null;

            var row = table.Rows[0];
            return new TaiKhoanModel
            {
                Id = row.Field<Guid>("Id"),
                Email = row.Field<string>("Email"),
                MatKhau = row.Field<string>("MatKhau"),
                SoDienThoai = row.Field<string>("SoDienThoai"),
                VaiTro = row.Field<string>("VaiTro"),
                TrangThaiHoatDong = row.Field<bool>("TrangThaiHoatDong"),
                CreatedAt = row.Field<DateTime>("CreatedAt"),
                UpdatedAt = row.Field<DateTime>("UpdatedAt")
            };
        }

        // 🔐 Đăng nhập
        public async Task<TaiKhoanModel?> LoginAsync(DangNhapModel model)
{
    using var conn = new SqlConnection(_connectionString);
    using var cmd = new SqlCommand("sp_TaiKhoan_Login", conn)
    {
        CommandType = CommandType.StoredProcedure
    };

    cmd.Parameters.AddWithValue("@TaiKhoan", model.TaiKhoan);
    cmd.Parameters.AddWithValue("@MatKhau", model.MatKhau);

    await conn.OpenAsync();
    using var reader = await cmd.ExecuteReaderAsync();

    var account = new TaiKhoanModel();

    // 1️⃣ Lấy bảng tài khoản chính
    if (await reader.ReadAsync())
    {
        account.Id = reader.GetGuid(0);
        account.Email = reader.GetString(1);
        account.MatKhau = reader.GetString(2);
        account.SoDienThoai = reader.GetString(3);
        account.VaiTro = reader.GetString(4);
        account.TrangThaiHoatDong = reader.GetBoolean(5);
    }
    else return null;

    // 2️⃣ Lấy bảng chi tiết theo vai trò
    if (await reader.NextResultAsync() && await reader.ReadAsync())
    {
        switch (account.VaiTro)
        {
            case "KHACH_HANG":
                account.ThongTinKhachHang = new KhachHangModel
                {
                    Id = reader.GetGuid(0),
                    TaiKhoanId = reader.GetGuid(1),
                    HoTen = reader.GetString(2),
                    DiaChiText = reader.IsDBNull(3) ? null : reader.GetString(3),
                    GhiChu = reader.IsDBNull(4) ? null : reader.GetString(4),
                    TrangThaiHoatDong = reader.GetBoolean(5),
                    Email = reader.GetString(6),
                    SoDienThoai = reader.GetString(7),
                    CreatedAt = reader.GetDateTime(8),
                    UpdatedAt = reader.GetDateTime(9)
                };
                break;

            case "DOANH_NGHIEP":
                account.ThongTinDoanhNghiep = new DoanhNghiepModel
                {
                    Id = reader.GetGuid(0),
                    TaiKhoanId = reader.GetGuid(1),
                    TenDoanhNghiep = reader.GetString(2),
                    MaSoThue = reader.IsDBNull(3) ? null : reader.GetString(3),
                    DiaChiText = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Website = reader.IsDBNull(5) ? null : reader.GetString(5),
                    MoTa = reader.IsDBNull(6) ? null : reader.GetString(6),
                    Verified = reader.GetBoolean(7)
                };
                break;

            case "NHAN_VIEN":
                account.ThongTinNhanVien = new NhanVienModel
                {
                    Id = reader.GetGuid(0),
                    TaiKhoanId = reader.GetGuid(1),
                    HoTen = reader.GetString(3),
                    TrangThaiSanSang = reader.GetBoolean(4),
                    TrangThaiHoatDong = reader.GetBoolean(5)
                };
                break;

            case "ADMIN":
                account.ThongTinAdmin = new AdminUserModel
                {
                    Id = reader.GetGuid(0),
                    TaiKhoanId = reader.GetGuid(1),
                    HoTen = reader.IsDBNull(2) ? null : reader.GetString(2),
                    GhiChu = reader.IsDBNull(3) ? null : reader.GetString(3),
                    TrangThaiHoatDong = reader.GetBoolean(4)
                };
                break;
        }
    }

    return account;
}


        // 📝 Đăng ký
        public async Task<(Guid TaiKhoanId, string Otp, string ThongBao)?> RegisterAsync(DangKyModel model)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_TaiKhoan_Register", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Email", model.Email);
            cmd.Parameters.AddWithValue("@MatKhau", model.MatKhau);
            cmd.Parameters.AddWithValue("@SoDienThoai", model.SoDienThoai);
            cmd.Parameters.AddWithValue("@VaiTro", model.VaiTro);
            cmd.Parameters.AddWithValue("@HoTen", (object?)model.HoTen ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DiaChiText", (object?)model.DiaChiText ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TenDoanhNghiep", (object?)model.TenDoanhNghiep ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MaSoThue", (object?)model.MaSoThue ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Website", (object?)model.Website ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@MoTa", (object?)model.MoTa ?? DBNull.Value);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return (
                    reader.GetGuid(0),
                    reader.GetString(2),
                    reader.GetString(3)
                );
            }

            return null;
        }

        // 🔑 Quên mật khẩu (lấy OTP)
        public async Task<(Guid TaiKhoanId, string Otp)?> ForgotPasswordAsync(string taiKhoan)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_TaiKhoan_QuenMatKhau", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@TaiKhoan", taiKhoan);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return (reader.GetGuid(0), reader.GetString(1));

            return null;
        }

        // 🔁 Đặt lại mật khẩu
        public async Task<bool> ResetPasswordAsync(Guid taiKhoanId, string otp, string matKhauMoi)
        {
            var parameters = new[] 
            {
                new SqlParameter("@TaiKhoanId", taiKhoanId),
                new SqlParameter("@OTP", otp),
                new SqlParameter("@MatKhauMoi", matKhauMoi)
            };

            try
            {
                await _db.ExecuteNonQueryAsync("sp_TaiKhoan_ResetPassword", parameters);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ✉️ Gửi OTP qua Gmail
        public async Task<bool> SendOtpEmailAsync(string email, string otp)
        {
            try
            {
                using var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new System.Net.NetworkCredential(_smtpEmail, _smtpPass)
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(_smtpEmail, "PheLieu System"),
                    Subject = "Mã OTP khôi phục mật khẩu",
                    Body = $"Mã OTP của bạn là: {otp}\nMã có hiệu lực 10 phút.",
                    IsBodyHtml = false
                };

                mail.To.Add(email);
                await smtp.SendMailAsync(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ➕ Thêm tài khoản
        public async Task<int> InsertAsync(TaiKhoanModel model)
        {
            var parameters = new[]
            {
                new SqlParameter("@Email", model.Email),
                new SqlParameter("@MatKhau", model.MatKhau),
                new SqlParameter("@SoDienThoai", model.SoDienThoai),
                new SqlParameter("@VaiTro", model.VaiTro)
            };
            return await _db.ExecuteNonQueryAsync("sp_TaiKhoan_Insert", parameters);
        }

        // ✏️ Cập nhật tài khoản
        public async Task<int> UpdateAsync(TaiKhoanModel model)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@Email", model.Email),
                new SqlParameter("@MatKhau", model.MatKhau),
                new SqlParameter("@SoDienThoai", model.SoDienThoai),
                new SqlParameter("@VaiTro", model.VaiTro)
            };
            return await _db.ExecuteNonQueryAsync("sp_TaiKhoan_Update", parameters);
        }

        // ❌ Xóa tài khoản
        public async Task<int> DeleteAsync(Guid id)
        {
            var parameters = new[] { new SqlParameter("@Id", id) };
            return await _db.ExecuteNonQueryAsync("sp_TaiKhoan_Delete", parameters);
        }
    }
}
