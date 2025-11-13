using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Helpers;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class ThongBaoHeThongService
    {
        private readonly DbHelper _db;
        private readonly string _connectionString;

        public ThongBaoHeThongService(DbHelper db, IConfiguration config)
        {
            _db = db;
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // Thêm thông báo hệ thống
        public async Task<int> InsertAsync(ThongBaoHeThongModel model)
        {
            var parameters = new[]
            {
                new SqlParameter("@TaiKhoanId", model.TaiKhoanId),
                new SqlParameter("@NoiDung", model.NoiDung),
                new SqlParameter("@DaDoc", model.DaDoc),
                new SqlParameter("@NgayGui", model.NgayGui)
            };
            return await _db.ExecuteNonQueryAsync("sp_ThongBaoHeThong_Insert", parameters);
        }

        // Lấy tất cả thông báo hệ thống
        public async Task<List<ThongBaoHeThongModel>> GetAllAsync()
        {
            var table = await _db.ExecuteQueryAsync("sp_ThongBaoHeThong_GetAll");
            return table.AsEnumerable().Select(row => new ThongBaoHeThongModel
            {
                Id = row.Field<Guid>("Id"),
                TaiKhoanId = row.Field<Guid>("TaiKhoanId"),
                NoiDung = row.Field<string>("NoiDung"),
                DaDoc = row.Field<bool>("DaDoc"),
                NgayGui = row.Field<DateTime>("NgayGui")
            }).ToList();
        }

        // Lấy thông báo theo ID
        public async Task<ThongBaoHeThongModel?> GetByIdAsync(Guid id)
        {
            var parameters = new[] { new SqlParameter("@Id", id) };
            var table = await _db.ExecuteQueryAsync("sp_ThongBaoHeThong_GetById", parameters);
            if (table.Rows.Count == 0) return null;

            var row = table.Rows[0];
            return new ThongBaoHeThongModel
            {
                Id = row.Field<Guid>("Id"),
                TaiKhoanId = row.Field<Guid>("TaiKhoanId"),
                NoiDung = row.Field<string>("NoiDung"),
                DaDoc = row.Field<bool>("DaDoc"),
                NgayGui = row.Field<DateTime>("NgayGui")
            };
        }

        // Cập nhật trạng thái đã đọc
        public async Task<int> MarkAsReadAsync(Guid id)
        {
            var parameters = new[] { new SqlParameter("@Id", id) };
            return await _db.ExecuteNonQueryAsync("sp_ThongBaoHeThong_MarkAsRead", parameters);
        }
    }
}
