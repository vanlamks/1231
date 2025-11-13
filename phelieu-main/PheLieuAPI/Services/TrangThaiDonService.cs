using System.Data;
using PheLieuAPI.Helpers;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class TrangThaiDonService
    {
        private readonly DbHelper _dbHelper;

        public TrangThaiDonService(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // 🧩 Lấy toàn bộ danh sách trạng thái đơn
        public async Task<List<TrangThaiDonModel>> GetAllAsync()
        {
            var dataTable = await _dbHelper.ExecuteQueryAsync("sp_TrangThaiDon_GetAll");

            var list = new List<TrangThaiDonModel>();
            foreach (DataRow row in dataTable.Rows)
            {
                var model = new TrangThaiDonModel
                {
                    Code = row["Code"].ToString(),
                    Ten = row["Ten"].ToString()
                };
                list.Add(model);
            }
            return list;
        }
    }
}
