using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class ThongBaoService
    {
        private readonly string _connection;

        public ThongBaoService(IConfiguration config)
        {
            _connection = config.GetConnectionString("DefaultConnection");
        }

        // 🟢 Lấy thông báo theo doanh nghiệp
        public async Task<List<ThongBaoModel>> GetByDoanhNghiepAsync(Guid doanhNghiepId)
        {
            var list = new List<ThongBaoModel>();

            using var conn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_ThongBao_GetByDoanhNghiep", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@DoanhNghiepId", doanhNghiepId);

            await conn.OpenAsync();

            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new ThongBaoModel
                {
                    Id = rd.GetGuid(0),
                    NoiDung = rd.GetString(1),
                    Loai = rd.GetString(2),
                    DaXem = rd.GetBoolean(3),
                    CreatedAt = rd.GetDateTime(4),
                    TenPheLieu = !rd.IsDBNull(5) ? rd.GetString(5) : null,
                    TenNguoiDang = !rd.IsDBNull(6) ? rd.GetString(6) : null
                });
            }

            return list;
        }

        // 🔵 Tạo thông báo cho bài bán hoặc mua (nếu muốn gọi trực tiếp)
        public async Task<Guid> InsertAsync(ThongBaoCreateModel model)
        {
            using var conn = new SqlConnection(_connection);
            using var cmd = new SqlCommand(@"
                INSERT INTO ThongBao (Id, DoanhNghiepId, Loai, DonBanId, DonMuaId, NoiDung)
                OUTPUT INSERTED.Id
                VALUES (NEWID(), @DoanhNghiepId, @Loai, @DonBanId, @DonMuaId, @NoiDung)
            ", conn);

            cmd.Parameters.AddWithValue("@DoanhNghiepId", (object?)model.DoanhNghiepId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Loai", model.Loai);
            cmd.Parameters.AddWithValue("@DonBanId", (object?)model.DonBanId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DonMuaId", (object?)model.DonMuaId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@NoiDung", model.NoiDung);

            await conn.OpenAsync();
            return (Guid)await cmd.ExecuteScalarAsync();
        }

        // 🟣 Đánh dấu đã xem
        public async Task<int> MarkAsReadAsync(Guid id)
        {
            using var conn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_ThongBao_DanhDauDaXem", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        // ❌ Xoá thông báo
        public async Task<int> DeleteAsync(Guid id)
        {
            using var conn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("DELETE FROM ThongBao WHERE Id=@Id", conn);

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
