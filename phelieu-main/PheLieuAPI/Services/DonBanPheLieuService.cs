using System.Data;
using Microsoft.Data.SqlClient;

namespace PheLieuAPI.Services
{
    public class DonBanPheLieuService
    {
        private readonly string _connectionString;

        public DonBanPheLieuService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // 🟢 Lấy tất cả
        public async Task<DataTable> GetAllAsync()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonBanPheLieu_GetAll", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            dt.Load(reader);
            return dt;
        }

        // 🔍 Lấy theo Id
        public async Task<DataTable> GetByIdAsync(Guid id)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonBanPheLieu_GetById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            dt.Load(reader);
            return dt;
        }

        // ➕ Thêm mới
        public async Task<int> InsertAsync(Guid? khachHangId, Guid? doanhNghiepId, string tenPheLieu, decimal khoiLuong, decimal donGia, string moTa)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonBanPheLieu_Insert", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@KhachHangId", (object?)khachHangId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DoanhNghiepId", (object?)doanhNghiepId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TenPheLieu", tenPheLieu);
            cmd.Parameters.AddWithValue("@KhoiLuong", khoiLuong);
            cmd.Parameters.AddWithValue("@DonGia", donGia);
            cmd.Parameters.AddWithValue("@MoTa", moTa ?? "");

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        // ✏️ Cập nhật
        public async Task<int> UpdateAsync(Guid id, decimal khoiLuong, decimal donGia, string moTa, string trangThai)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonBanPheLieu_Update", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@KhoiLuong", khoiLuong);
            cmd.Parameters.AddWithValue("@DonGia", donGia);
            cmd.Parameters.AddWithValue("@MoTa", moTa ?? "");
            cmd.Parameters.AddWithValue("@TrangThai", trangThai);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        // 🗑️ Xóa đơn
        public async Task<int> DeleteAsync(Guid id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonBanPheLieu_Delete", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
