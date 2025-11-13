using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class PheLieuService
    {
        private readonly string _connectionString;

        public PheLieuService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<PheLieuModel>> GetAllAsync()
        {
            var list = new List<PheLieuModel>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_PheLieu_GetAll", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new PheLieuModel
                {
                    Id = (Guid)reader["Id"],
                    TenPheLieu = reader["TenPheLieu"].ToString(),
                    MaLoai = reader["MaLoai"].ToString(),
                    TenLoai = reader["TenLoai"].ToString(),
                    KhoiLuong = (decimal)reader["KhoiLuong"],
                    DonGia = (decimal)reader["DonGia"],
                    MoTa = reader["MoTa"]?.ToString(),
                    HinhAnh = reader["HinhAnh"]?.ToString(),
                    CreatedAt = (DateTime)reader["CreatedAt"],
                    UpdatedAt = (DateTime)reader["UpdatedAt"]
                });
            }

            return list;
        }

        public async Task<int> InsertAsync(PheLieuModel model)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_PheLieu_Insert", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@TenPheLieu", model.TenPheLieu);
            cmd.Parameters.AddWithValue("@MaLoai", model.MaLoai);
            cmd.Parameters.AddWithValue("@KhoiLuong", model.KhoiLuong);
            cmd.Parameters.AddWithValue("@DonGia", model.DonGia);
            cmd.Parameters.AddWithValue("@MoTa", model.MoTa ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@HinhAnh", model.HinhAnh ?? (object)DBNull.Value);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> UpdateAsync(PheLieuModel model)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_PheLieu_Update", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", model.Id);
            cmd.Parameters.AddWithValue("@TenPheLieu", model.TenPheLieu);
            cmd.Parameters.AddWithValue("@MaLoai", model.MaLoai);
            cmd.Parameters.AddWithValue("@KhoiLuong", model.KhoiLuong);
            cmd.Parameters.AddWithValue("@DonGia", model.DonGia);
            cmd.Parameters.AddWithValue("@MoTa", model.MoTa ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@HinhAnh", model.HinhAnh ?? (object)DBNull.Value);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_PheLieu_Delete", conn)
            {
                CommandType = CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
