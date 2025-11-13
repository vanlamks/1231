using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class DonThuGomService
    {
        private readonly string _connectionString;
        public DonThuGomService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<List<DonThuGomModel>> GetAllAsync()
        {
            var list = new List<DonThuGomModel>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonThuGom_GetAll", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new DonThuGomModel
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    TrangThaiCode = reader["TrangThaiCode"].ToString() ?? "",
                    TongTien = reader.GetDecimal(reader.GetOrdinal("TongTien")),
                    PhuongThucTT = reader["PhuongThucTT"].ToString() ?? "",
                    GhiChu = reader["GhiChu"].ToString(),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt")),
                    TenNhanVien = reader["TenNhanVien"].ToString(),
                    TenDoanhNghiep = reader["TenDoanhNghiep"].ToString()
                });
            }
            return list;
        }

        public async Task<int> InsertAsync(DonThuGomModel model)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonThuGom_Insert", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@LichHenId", model.LichHenId);
            cmd.Parameters.AddWithValue("@NhanVienId", model.NhanVienId);
            cmd.Parameters.AddWithValue("@DoanhNghiepId", model.DoanhNghiepId);
            cmd.Parameters.AddWithValue("@TrangThaiCode", model.TrangThaiCode);
            cmd.Parameters.AddWithValue("@TongTien", model.TongTien);
            cmd.Parameters.AddWithValue("@PhuongThucTT", model.PhuongThucTT);
            cmd.Parameters.AddWithValue("@GhiChu", model.GhiChu ?? "");
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> UpdateAsync(DonThuGomModel model)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonThuGom_Update", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@TrangThaiCode", model.TrangThaiCode);
            cmd.Parameters.AddWithValue("@TongTien", model.TongTien);
            cmd.Parameters.AddWithValue("@PhuongThucTT", model.PhuongThucTT);
            cmd.Parameters.AddWithValue("@GhiChu", model.GhiChu ?? "");
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonThuGom_Delete", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> UpdateTrangThaiAsync(Guid id, string trangThaiCode, string? ghiChu)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonThuGom_UpdateTrangThai", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@TrangThaiCode", trangThaiCode);
            cmd.Parameters.AddWithValue("@GhiChu", ghiChu ?? "");
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
