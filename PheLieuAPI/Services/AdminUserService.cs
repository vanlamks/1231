using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Helpers;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class AdminUserService
    {
        private readonly DbHelper _db;

        public AdminUserService(DbHelper db)
        {
            _db = db;
        }

        // 🧩 Lấy danh sách admin
        public async Task<List<AdminUserModel>> GetAllAsync()
        {
            var table = await _db.ExecuteQueryAsync("sp_Admin_GetAll");
            return table.AsEnumerable().Select(row => new AdminUserModel
            {
                Id = row.Field<Guid>("Id"),
                TaiKhoanId = row.Field<Guid>("TaiKhoanId"),
                HoTen = row.Field<string?>("HoTen"),
                GhiChu = row.Field<string?>("GhiChu"),
                TrangThaiHoatDong = row.Field<bool>("TrangThaiHoatDong"),
                CreatedAt = row.Field<DateTime>("CreatedAt"),
                UpdatedAt = row.Field<DateTime>("UpdatedAt")
            }).ToList();
        }

        // 🔍 Lấy theo ID
        public async Task<AdminUserModel?> GetByIdAsync(Guid id)
        {
            var parameters = new[] { new SqlParameter("@Id", id) };
            var table = await _db.ExecuteQueryAsync("sp_Admin_GetAll", parameters);
            if (table.Rows.Count == 0) return null;

            var row = table.Rows[0];
            return new AdminUserModel
            {
                Id = row.Field<Guid>("Id"),
                TaiKhoanId = row.Field<Guid>("TaiKhoanId"),
                HoTen = row.Field<string?>("HoTen"),
                GhiChu = row.Field<string?>("GhiChu"),
                TrangThaiHoatDong = row.Field<bool>("TrangThaiHoatDong"),
                CreatedAt = row.Field<DateTime>("CreatedAt"),
                UpdatedAt = row.Field<DateTime>("UpdatedAt")
            };
        }

        // ➕ Thêm admin
        public async Task<int> InsertAsync(AdminUserModel model)
        {
            var parameters = new[]
            {
                new SqlParameter("@TaiKhoanId", model.TaiKhoanId),
                new SqlParameter("@HoTen", (object?)model.HoTen ?? DBNull.Value),
                new SqlParameter("@GhiChu", (object?)model.GhiChu ?? DBNull.Value)
            };

            return await _db.ExecuteNonQueryAsync("sp_Admin_Insert", parameters);
        }

        // ✏️ Cập nhật admin
        public async Task<int> UpdateAsync(AdminUserModel model)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@HoTen", (object?)model.HoTen ?? DBNull.Value),
                new SqlParameter("@GhiChu", (object?)model.GhiChu ?? DBNull.Value)
            };

            return await _db.ExecuteNonQueryAsync("sp_Admin_Update", parameters);
        }

        // ❌ Xóa mềm
        public async Task<int> DeleteAsync(Guid id)
        {
            var parameters = new[] { new SqlParameter("@Id", id) };
            return await _db.ExecuteNonQueryAsync("sp_Admin_Delete", parameters);
        }
    }
}
