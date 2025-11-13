using System.Data;
using Microsoft.Data.SqlClient;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class TrangThaiLichHenService
    {
        private readonly string _connectionString;

        public TrangThaiLichHenService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<List<TrangThaiLichHenModel>> GetAllAsync()
        {
            var list = new List<TrangThaiLichHenModel>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_TrangThaiLichHen_GetAll", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new TrangThaiLichHenModel
                {
                    Code = reader["Code"].ToString() ?? "",
                    Ten = reader["Ten"].ToString() ?? ""
                });
            }

            return list;
        }
    }
}
