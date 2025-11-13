using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Helpers;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class NhanVienService
    {
        private readonly DbHelper _db;

        public NhanVienService(DbHelper db)
        {
            _db = db;
        }

        // 🧠 Lấy danh sách toàn bộ nhân viên
        public async Task<List<NhanVienModel>> GetAllAsync()
        {
            var table = await _db.ExecuteQueryAsync("sp_NhanVien_GetAll");

            return table.AsEnumerable().Select(row => new NhanVienModel
            {
                Id = row.Field<Guid>("Id"),
                TaiKhoanId = row.Field<Guid>("TaiKhoanId"),
                DoanhNghiepId = row.Field<Guid?>("DoanhNghiepId"),
                HoTen = row.Field<string>("HoTen"),
                TrangThaiSanSang = row.Field<bool>("TrangThaiSanSang"),
                TrangThaiHoatDong = row.Field<bool>("TrangThaiHoatDong"),
                TenDoanhNghiep = row.Field<string?>("TenDoanhNghiep"),
                Email = row.Field<string?>("Email"),
                CreatedAt = row.Field<DateTime>("CreatedAt"),
                UpdatedAt = row.Field<DateTime>("UpdatedAt")
            }).ToList();
        }

        // 🔍 Lấy theo ID
        public async Task<NhanVienModel?> GetByIdAsync(Guid id)
        {
            var parameters = new[] { new SqlParameter("@Id", id) };
            var table = await _db.ExecuteQueryAsync("sp_NhanVien_GetById", parameters);
            if (table.Rows.Count == 0) return null;

            var row = table.Rows[0];
            return new NhanVienModel
            {
                Id = row.Field<Guid>("Id"),
                TaiKhoanId = row.Field<Guid>("TaiKhoanId"),
                DoanhNghiepId = row.Field<Guid?>("DoanhNghiepId"),
                HoTen = row.Field<string>("HoTen"),
                TrangThaiSanSang = row.Field<bool>("TrangThaiSanSang"),
                TrangThaiHoatDong = row.Field<bool>("TrangThaiHoatDong"),
                TenDoanhNghiep = row.Field<string?>("TenDoanhNghiep"),
                Email = row.Field<string?>("Email"),
                CreatedAt = row.Field<DateTime>("CreatedAt"),
                UpdatedAt = row.Field<DateTime>("UpdatedAt")
            };
        }

        // ➕ Thêm nhân viên
        public async Task<int> InsertAsync(NhanVienModel model)
        {
            var parameters = new[]
            {
                new SqlParameter("@TaiKhoanId", model.TaiKhoanId),
                new SqlParameter("@DoanhNghiepId", (object?)model.DoanhNghiepId ?? DBNull.Value),
                new SqlParameter("@HoTen", model.HoTen)
            };

            return await _db.ExecuteNonQueryAsync("sp_NhanVien_Insert", parameters);
        }

        // ✏️ Cập nhật
        public async Task<int> UpdateAsync(NhanVienModel model)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@DoanhNghiepId", (object?)model.DoanhNghiepId ?? DBNull.Value),
                new SqlParameter("@HoTen", model.HoTen),
                new SqlParameter("@TrangThaiSanSang", model.TrangThaiSanSang)
            };

            return await _db.ExecuteNonQueryAsync("sp_NhanVien_Update", parameters);
        }

        // ❌ Xóa mềm
        public async Task<int> DeleteAsync(Guid id)
        {
            var parameters = new[] { new SqlParameter("@Id", id) };
            return await _db.ExecuteNonQueryAsync("sp_NhanVien_Delete", parameters);
        }
    }
}
