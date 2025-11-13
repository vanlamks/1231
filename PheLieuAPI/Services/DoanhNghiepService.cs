using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Helpers;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class DoanhNghiepService
    {
        private readonly DbHelper _db;

        public DoanhNghiepService(DbHelper db)
        {
            _db = db;
        }

        // 🧾 Lấy danh sách doanh nghiệp
        public async Task<List<DoanhNghiepModel>> GetAllAsync()
        {
            var table = await _db.ExecuteQueryAsync("sp_DoanhNghiep_GetAll");
            return table.AsEnumerable().Select(row => new DoanhNghiepModel
            {
                Id = row.Field<Guid>("Id"),
                TaiKhoanId = row.Field<Guid>("TaiKhoanId"),
                TenDoanhNghiep = row.Field<string>("TenDoanhNghiep"),
                MaSoThue = row.Field<string?>("MaSoThueGiaiMa"),
                DiaChiText = row.Field<string?>("DiaChiText"),
                Website = row.Field<string?>("Website"),
                MoTa = row.Field<string?>("MoTa"),
                Verified = row.Field<bool>("Verified"),
                TrangThaiHoatDong = row.Field<bool>("TrangThaiHoatDong"),
                CreatedAt = row.Field<DateTime>("CreatedAt"),
                UpdatedAt = row.Field<DateTime>("UpdatedAt")
            }).ToList();
        }

        // 🔍 Lấy theo ID
        public async Task<DoanhNghiepModel?> GetByIdAsync(Guid id)
        {
            var parameters = new[] { new SqlParameter("@Id", id) };
            var table = await _db.ExecuteQueryAsync("sp_DoanhNghiep_GetById", parameters);
            if (table.Rows.Count == 0) return null;

            var row = table.Rows[0];
            return new DoanhNghiepModel
            {
                Id = row.Field<Guid>("Id"),
                TaiKhoanId = row.Field<Guid>("TaiKhoanId"),
                TenDoanhNghiep = row.Field<string>("TenDoanhNghiep"),
                MaSoThue = row.Field<string?>("MaSoThueGiaiMa"),
                DiaChiText = row.Field<string?>("DiaChiText"),
                Website = row.Field<string?>("Website"),
                MoTa = row.Field<string?>("MoTa"),
                Verified = row.Field<bool>("Verified"),
                TrangThaiHoatDong = row.Field<bool>("TrangThaiHoatDong"),
                CreatedAt = row.Field<DateTime>("CreatedAt"),
                UpdatedAt = row.Field<DateTime>("UpdatedAt")
            };
        }

        // ➕ Thêm doanh nghiệp
        public async Task<int> InsertAsync(DoanhNghiepModel model)
        {
            var parameters = new[]
            {
                new SqlParameter("@TaiKhoanId", model.TaiKhoanId),
                new SqlParameter("@TenDoanhNghiep", model.TenDoanhNghiep),
                new SqlParameter("@MaSoThue", (object?)model.MaSoThue ?? DBNull.Value),
                new SqlParameter("@DiaChiText", (object?)model.DiaChiText ?? DBNull.Value),
                new SqlParameter("@Website", (object?)model.Website ?? DBNull.Value),
                new SqlParameter("@MoTa", (object?)model.MoTa ?? DBNull.Value)
            };

            return await _db.ExecuteNonQueryAsync("sp_DoanhNghiep_Insert", parameters);
        }

        // ✏️ Cập nhật
        public async Task<int> UpdateAsync(DoanhNghiepModel model)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@TenDoanhNghiep", model.TenDoanhNghiep),
                new SqlParameter("@MaSoThue", (object?)model.MaSoThue ?? DBNull.Value),
                new SqlParameter("@DiaChiText", (object?)model.DiaChiText ?? DBNull.Value),
                new SqlParameter("@Website", (object?)model.Website ?? DBNull.Value),
                new SqlParameter("@MoTa", (object?)model.MoTa ?? DBNull.Value)
            };

            return await _db.ExecuteNonQueryAsync("sp_DoanhNghiep_Update", parameters);
        }

        // ❌ Xóa mềm
        public async Task<int> DeleteAsync(Guid id)
        {
            var parameters = new[] { new SqlParameter("@Id", id) };
            return await _db.ExecuteNonQueryAsync("sp_DoanhNghiep_Delete", parameters);
        }
    }
}
