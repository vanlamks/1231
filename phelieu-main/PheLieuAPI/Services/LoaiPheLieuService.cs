using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class LoaiPheLieuService
    {
        private readonly string _connectionString;

        public LoaiPheLieuService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // 🧩 Lấy tất cả
        public async Task<List<LoaiPheLieuModel>> GetAllAsync()
        {
            var list = new List<LoaiPheLieuModel>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_LoaiPheLieu_GetAll", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new LoaiPheLieuModel
                {
                    MaLoai = reader["MaLoai"].ToString() ?? "",
                    TenLoai = reader["TenLoai"].ToString() ?? "",
                    MoTa = reader["MoTa"]?.ToString()
                });
            }

            return list;
        }

        // 🧩 Lấy theo mã
        public async Task<LoaiPheLieuModel?> GetByIdAsync(string maLoai)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_LoaiPheLieu_GetById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@MaLoai", maLoai);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new LoaiPheLieuModel
                {
                    MaLoai = reader["MaLoai"].ToString() ?? "",
                    TenLoai = reader["TenLoai"].ToString() ?? "",
                    MoTa = reader["MoTa"]?.ToString()
                };
            }

            return null;
        }

        // ➕ Thêm
        public async Task<int> InsertAsync(LoaiPheLieuModel model)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_LoaiPheLieu_Insert", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@MaLoai", model.MaLoai);
            cmd.Parameters.AddWithValue("@TenLoai", model.TenLoai);
            cmd.Parameters.AddWithValue("@MoTa", (object?)model.MoTa ?? DBNull.Value);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        // ✏️ Cập nhật
        public async Task<int> UpdateAsync(LoaiPheLieuModel model)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_LoaiPheLieu_Update", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@MaLoai", model.MaLoai);
            cmd.Parameters.AddWithValue("@TenLoai", model.TenLoai);
            cmd.Parameters.AddWithValue("@MoTa", (object?)model.MoTa ?? DBNull.Value);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        // ❌ Xóa
        public async Task<int> DeleteAsync(string maLoai)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_LoaiPheLieu_Delete", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@MaLoai", maLoai);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
