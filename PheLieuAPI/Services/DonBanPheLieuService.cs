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

        // ➕ Thêm mới – TRẢ VỀ Id VỪA TẠO
        public async Task<Guid?> InsertAsync(
            Guid? khachHangId,
            Guid? doanhNghiepId,
            string tenPheLieu,
            decimal khoiLuong,
            decimal donGia,
            string moTa)
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
            cmd.Parameters.AddWithValue("@MoTa", moTa ?? string.Empty);

            await conn.OpenAsync();

            // SP SELECT @NewId AS Id nên dùng reader để lấy Guid
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                // có thể dùng reader.GetGuid(0) hoặc theo tên cột:
                // return (Guid)reader["Id"];
                return reader.GetGuid(0);
            }

            return null;
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
            cmd.Parameters.AddWithValue("@MoTa", moTa ?? string.Empty);
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
