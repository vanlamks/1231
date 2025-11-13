using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class LichHenService
    {
        private readonly string _connectionString;

        public LichHenService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // 🧩 GET ALL
        public async Task<List<LichHenModel>> GetAllAsync()
        {
            var list = new List<LichHenModel>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_LichHen_GetAll", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new LichHenModel
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    KhachHangId = reader.GetGuid(reader.GetOrdinal("KhachHangId")),
                    TenKhachHang = reader["TenKhachHang"]?.ToString(),
                    DiaChi = reader["DiaChi"]?.ToString(),
                    ThoiGianHen = reader.GetDateTime(reader.GetOrdinal("ThoiGianHen")),
                    TrangThaiCode = reader["TrangThaiCode"].ToString() ?? "",
                    TenTrangThai = reader["TenTrangThai"]?.ToString(),
                    GhiChu = reader["GhiChu"]?.ToString(),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                });
            }

            return list;
        }

        // 🧩 GET BY ID
        public async Task<LichHenModel?> GetByIdAsync(Guid id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_LichHen_GetById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new LichHenModel
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    KhachHangId = reader.GetGuid(reader.GetOrdinal("KhachHangId")),
                    TenKhachHang = reader["TenKhachHang"]?.ToString(),
                    DiaChi = reader["DiaChi"]?.ToString(),
                    ThoiGianHen = reader.GetDateTime(reader.GetOrdinal("ThoiGianHen")),
                    TrangThaiCode = reader["TrangThaiCode"].ToString() ?? "",
                    TenTrangThai = reader["TenTrangThai"]?.ToString(),
                    GhiChu = reader["GhiChu"]?.ToString(),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
                };
            }

            return null;
        }

        // ➕ INSERT
        public async Task<int> InsertAsync(LichHenModel model)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_LichHen_Insert", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@KhachHangId", model.KhachHangId);
            cmd.Parameters.AddWithValue("@DiaChi", model.DiaChi);
            cmd.Parameters.AddWithValue("@ThoiGianHen", model.ThoiGianHen);
            cmd.Parameters.AddWithValue("@TrangThaiCode", model.TrangThaiCode);
            cmd.Parameters.AddWithValue("@GhiChu", (object?)model.GhiChu ?? DBNull.Value);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        // ✏️ UPDATE
        public async Task<int> UpdateAsync(LichHenModel model)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_LichHen_Update", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@TrangThaiCode", model.TrangThaiCode);
            cmd.Parameters.AddWithValue("@GhiChu", (object?)model.GhiChu ?? DBNull.Value);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        // ❌ DELETE
        public async Task<int> DeleteAsync(Guid id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_LichHen_Delete", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
