using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Helpers;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class ViTriNguoiDungService
    {
        private readonly DbHelper _dbHelper;

        public ViTriNguoiDungService(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public async Task<List<ViTriNguoiDungModel>> GetAllAsync()
        {
            var dataTable = await _dbHelper.ExecuteQueryAsync("sp_ViTriNguoiDung_GetAll");
            var list = new List<ViTriNguoiDungModel>();

            foreach (DataRow row in dataTable.Rows)
            {
                list.Add(new ViTriNguoiDungModel
                {
                    Id = Guid.Parse(row["Id"].ToString()),
                    TaiKhoanId = Guid.Parse(row["TaiKhoanId"].ToString()),
                    KinhDo = Convert.ToDouble(row["KinhDo"]),
                    ViDo = Convert.ToDouble(row["ViDo"]),
                    ThoiGianCapNhat = Convert.ToDateTime(row["ThoiGianCapNhat"]),
                    Email = row["Email"].ToString()
                });
            }

            return list;
        }

        public async Task<int> InsertAsync(ViTriNguoiDungModel model)
        {
            var parameters = new[]
            {
                new SqlParameter("@TaiKhoanId", model.TaiKhoanId),
                new SqlParameter("@KinhDo", model.KinhDo),
                new SqlParameter("@ViDo", model.ViDo)
            };
            return await _dbHelper.ExecuteNonQueryAsync("sp_ViTriNguoiDung_Insert", parameters);
        }

        public async Task<int> UpdateAsync(ViTriNguoiDungModel model)
        {
            var parameters = new[]
            {
                new SqlParameter("@TaiKhoanId", model.TaiKhoanId),
                new SqlParameter("@KinhDo", model.KinhDo),
                new SqlParameter("@ViDo", model.ViDo)
            };
            return await _dbHelper.ExecuteNonQueryAsync("sp_ViTriNguoiDung_Update", parameters);
        }

        public async Task<int> DeleteAsync(Guid taiKhoanId)
        {
            var parameters = new[]
            {
                new SqlParameter("@TaiKhoanId", taiKhoanId)
            };
            return await _dbHelper.ExecuteNonQueryAsync("sp_ViTriNguoiDung_Delete", parameters);
        }
    }
}
