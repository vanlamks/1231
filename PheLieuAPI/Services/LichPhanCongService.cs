using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Helpers;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class LichPhanCongService
    {
        private readonly DbHelper _db;

        public LichPhanCongService(DbHelper db)
        {
            _db = db;
        }

        // 🔄 Chuyển DataTable → List<Dictionary<string, object>>
        private List<Dictionary<string, object>> ConvertToList(DataTable dt)
        {
            var list = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                var dict = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    dict[col.ColumnName] = row[col];
                }
                list.Add(dict);
            }
            return list;
        }

        // ✅ Lấy lịch theo Doanh nghiệp
        public async Task<List<Dictionary<string, object>>> GetByDoanhNghiepAsync(Guid doanhNghiepId)
        {
            string sql = @"SELECT * 
                           FROM LichPhanCong 
                           WHERE DoanhNghiepId=@Id 
                           ORDER BY CreatedAt DESC";

            var dt = new DataTable();
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", doanhNghiepId);
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                dt.Load(reader);
            }

            return ConvertToList(dt);
        }

        // ✅ Lấy lịch theo Nhân viên
        public async Task<List<Dictionary<string, object>>> GetByNhanVienAsync(Guid nhanVienId)
        {
            string sql = @"SELECT * 
                           FROM LichPhanCong 
                           WHERE NhanVienId=@Id 
                           ORDER BY CreatedAt DESC";

            var dt = new DataTable();
            using (var conn = _db.GetConnection())
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", nhanVienId);
                await conn.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                dt.Load(reader);
            }

            return ConvertToList(dt);
        }

        // ✅ Tự động tạo phân công (SP)
        public async Task<int> InsertAutoAsync(LichPhanCongModel m)
        {
            var parameters = new SqlParameter[]
            {
                new("@DoanhNghiepId", m.DoanhNghiepId),
                new("@CongViec", m.CongViec),
                new("@DiaDiem", m.DiaDiem),
                new("@KinhDo", m.KinhDo),
                new("@ViDo", m.ViDo),
                new("@ThoiGianBatDau", m.ThoiGianBatDau),
                new("@ThoiGianKetThuc", m.ThoiGianKetThuc)
            };

            return await _db.ExecuteNonQueryAsync("sp_LichPhanCong_InsertAuto", parameters);
        }

        // ✅ Nhân viên nhận việc
        public async Task<int> NhanAsync(Guid id)
        {
            var p = new SqlParameter("@Id", id);
            return await _db.ExecuteNonQueryAsync("sp_LichPhanCong_Nhan", new[] { p });
        }

        // ✅ Nhân viên từ chối
        public async Task<int> TuChoiAsync(Guid id)
        {
            var p = new SqlParameter("@Id", id);
            return await _db.ExecuteNonQueryAsync("sp_LichPhanCong_TuChoi", new[] { p });
        }

        // ✅ Xóa lịch
        public async Task<int> DeleteAsync(Guid id)
        {
            string sql = "DELETE FROM LichPhanCong WHERE Id=@Id";
            using var conn = _db.GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", id);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
