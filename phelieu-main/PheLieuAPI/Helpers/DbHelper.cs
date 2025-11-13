using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace PheLieuAPI.Helpers
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                                ?? throw new ArgumentNullException("Connection string 'DefaultConnection' not found!");
        }

        // ✅ Hàm mở kết nối
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // ✅ Gọi thủ tục có SELECT (trả về DataTable)
        public async Task<DataTable> ExecuteQueryAsync(string procedure, SqlParameter[]? parameters = null)
        {
            var dt = new DataTable();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(procedure, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters is not null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    dt.Load(reader);
                }
            }

            return dt;
        }

        // ✅ Gọi thủ tục INSERT / UPDATE / DELETE
        public async Task<int> ExecuteNonQueryAsync(string procedure, SqlParameter[]? parameters = null)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(procedure, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters is not null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
