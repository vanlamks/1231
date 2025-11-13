using Microsoft.Data.SqlClient;
using System.Data;
using PheLieuAPI.Models;

namespace PheLieuAPI.Services
{
    public class PaymentMethodService
    {
        private readonly string _connectionString;

        public PaymentMethodService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public async Task<List<PaymentMethodModel>> GetAllAsync()
        {
            var list = new List<PaymentMethodModel>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT Code, Ten FROM PaymentMethod ORDER BY Ten", conn);
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new PaymentMethodModel
                {
                    Code = reader["Code"].ToString(),
                    Ten = reader["Ten"].ToString()
                });
            }
            return list;
        }
    }
}
