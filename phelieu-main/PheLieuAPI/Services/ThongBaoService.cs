using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Helpers;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class ThongBaoService
    {
        private readonly DbHelper _db;

        public ThongBaoService(DbHelper db)
        {
            _db = db;
        }

        // 🔹 Lấy danh sách thông báo theo doanh nghiệp
        public async Task<List<ThongBaoModel>> GetByDoanhNghiepAsync(Guid doanhNghiepId)
        {
            var parameters = new[]
            {
                new SqlParameter("@DoanhNghiepId", doanhNghiepId)
            };

            var table = await _db.ExecuteQueryAsync("sp_ThongBao_GetByDoanhNghiep", parameters);

            return table.AsEnumerable().Select(row => new ThongBaoModel
            {
                Id = row.Field<Guid>("Id"),
                DoanhNghiepId = doanhNghiepId,
                Loai = row.Field<string>("Loai"),
                NoiDung = row.Field<string>("NoiDung"),
                DaXem = row.Field<bool>("DaXem"),
                CreatedAt = row.Field<DateTime>("CreatedAt"),
                TenPheLieu = row.Table.Columns.Contains("TenBan") && !row.IsNull("TenBan")
                                ? row.Field<string>("TenBan")
                                : (row.Table.Columns.Contains("TenMua") && !row.IsNull("TenMua")
                                    ? row.Field<string>("TenMua")
                                    : null)
            }).ToList();
        }

        // 🔹 Đánh dấu thông báo đã xem
        public async Task<int> MarkAsReadAsync(Guid id)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", id)
            };

            return await _db.ExecuteNonQueryAsync("sp_ThongBao_DanhDauDaXem", parameters);
        }

        // 🔹 Xóa thông báo
        public async Task<int> DeleteAsync(Guid id)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", id)
            };
            return await _db.ExecuteNonQueryAsync("DELETE FROM ThongBao WHERE Id=@Id", parameters);
        }
    }
}
