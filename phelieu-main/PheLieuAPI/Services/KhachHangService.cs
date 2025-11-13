using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Helpers;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class KhachHangService
    {
        private readonly DbHelper _db;

        public KhachHangService(DbHelper db)
        {
            _db = db;
        }

        // 🔹 Lấy danh sách khách hàng (kèm Email, SĐT)
        public async Task<List<KhachHangModel>> GetAllAsync()
        {
            var dt = await _db.ExecuteQueryAsync("sp_KhachHang_GetAll");
            return dt.AsEnumerable().Select(r => new KhachHangModel
            {
                Id = r.Field<Guid>("Id"),
                TaiKhoanId = r.Field<Guid>("TaiKhoanId"),
                Email = r.Field<string?>("Email"),
                SoDienThoai = r.Field<string?>("SoDienThoai"),
                HoTen = r.Field<string>("HoTen"),
                DiaChiText = r.Field<string?>("DiaChiText"),
                GhiChu = r.Field<string?>("GhiChu"),
                TrangThaiHoatDong = r.Field<bool>("TrangThaiHoatDong"),
                CreatedAt = r.Field<DateTime>("CreatedAt"),
                UpdatedAt = r.Field<DateTime>("UpdatedAt")
            }).ToList();
        }

        // 🔹 Thêm mới khách hàng
        public async Task<int> InsertAsync(KhachHangModel model)
        {
            var pr = new[]
            {
                new SqlParameter("@TaiKhoanId", model.TaiKhoanId),
                new SqlParameter("@HoTen", model.HoTen),
                new SqlParameter("@DiaChiText", (object?)model.DiaChiText ?? DBNull.Value),
                new SqlParameter("@GhiChu", (object?)model.GhiChu ?? DBNull.Value)
            };
            return await _db.ExecuteNonQueryAsync("sp_KhachHang_Insert", pr);
        }

        // 🔹 Cập nhật khách hàng
        public async Task<int> UpdateAsync(KhachHangModel model)
        {
            var pr = new[]
            {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@HoTen", model.HoTen),
                new SqlParameter("@DiaChiText", (object?)model.DiaChiText ?? DBNull.Value),
                new SqlParameter("@GhiChu", (object?)model.GhiChu ?? DBNull.Value)
            };
            return await _db.ExecuteNonQueryAsync("sp_KhachHang_Update", pr);
        }

        // 🔹 Xóa mềm khách hàng
        public async Task<int> DeleteAsync(Guid id)
        {
            var pr = new[] { new SqlParameter("@Id", id) };
            return await _db.ExecuteNonQueryAsync("sp_KhachHang_Delete", pr);
        }
    }
}
