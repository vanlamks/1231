using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Helpers;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class LichSuViTriNhanVienService
    {
        private readonly DbHelper _dbHelper;

        public LichSuViTriNhanVienService(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<List<LichSuViTriNhanVienModel>> GetAllAsync()
        {
            var dt = await _dbHelper.ExecuteQueryAsync("sp_LichSuViTriNhanVien_GetAll");
            var list = new List<LichSuViTriNhanVienModel>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new LichSuViTriNhanVienModel
                {
                    Id = Guid.Parse(row["Id"].ToString()),
                    NhanVienId = Guid.Parse(row["NhanVienId"].ToString()),
                    KinhDo = Convert.ToDouble(row["KinhDo"]),
                    ViDo = Convert.ToDouble(row["ViDo"]),
                    ThoiGian = Convert.ToDateTime(row["ThoiGian"]),
                    TenNhanVien = row["TenNhanVien"].ToString()
                });
            }

            return list;
        }

        public async Task<List<LichSuViTriNhanVienModel>> GetByNhanVienAsync(Guid nhanVienId)
        {
            var parameters = new[] { new SqlParameter("@NhanVienId", nhanVienId) };
            var dt = await _dbHelper.ExecuteQueryAsync("sp_LichSuViTriNhanVien_GetByNhanVien", parameters);
            var list = new List<LichSuViTriNhanVienModel>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new LichSuViTriNhanVienModel
                {
                    Id = Guid.Parse(row["Id"].ToString()),
                    NhanVienId = Guid.Parse(row["NhanVienId"].ToString()),
                    KinhDo = Convert.ToDouble(row["KinhDo"]),
                    ViDo = Convert.ToDouble(row["ViDo"]),
                    ThoiGian = Convert.ToDateTime(row["ThoiGian"]),
                    TenNhanVien = row["TenNhanVien"].ToString()
                });
            }

            return list;
        }

        public async Task<int> InsertAsync(LichSuViTriNhanVienModel model)
        {
            var parameters = new[]
            {
                new SqlParameter("@NhanVienId", model.NhanVienId),
                new SqlParameter("@KinhDo", model.KinhDo),
                new SqlParameter("@ViDo", model.ViDo)
            };
            return await _dbHelper.ExecuteNonQueryAsync("sp_LichSuViTriNhanVien_Insert", parameters);
        }

        public async Task<int> UpdateAsync(LichSuViTriNhanVienModel model)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@KinhDo", model.KinhDo),
                new SqlParameter("@ViDo", model.ViDo)
            };
            return await _dbHelper.ExecuteNonQueryAsync("sp_LichSuViTriNhanVien_Update", parameters);
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            var parameters = new[] { new SqlParameter("@Id", id) };
            return await _dbHelper.ExecuteNonQueryAsync("sp_LichSuViTriNhanVien_Delete", parameters);
        }
    }
}
