using System.Data;
using Microsoft.Data.SqlClient;

namespace PheLieuAPI.Services
{
    public class DonMuaPheLieuService
    {
        private readonly string _connectionString;

        public DonMuaPheLieuService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // 🟢 Lấy tất cả đơn mua
        public async Task<DataTable> GetAllAsync()
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonMuaPheLieu_GetAll", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            dt.Load(reader);

            return dt;
        }

        // 🔍 Lấy chi tiết theo Id
        public async Task<DataTable> GetByIdAsync(Guid id)
        {
            var dt = new DataTable();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonMuaPheLieu_GetById", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            dt.Load(reader);

            return dt;
        }

        // ➕ TẠO ĐƠN MUA — TRẢ VỀ ID MỚI
        public async Task<Guid?> InsertAsync(
            Guid? khachHangId,
            Guid? doanhNghiepId,
            string tenPheLieu,
            decimal khoiLuong,
            decimal donGiaDeXuat,
            string moTa)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonMuaPheLieu_Insert", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@KhachHangId", (object?)khachHangId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@DoanhNghiepId", (object?)doanhNghiepId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TenPheLieu", tenPheLieu);
            cmd.Parameters.AddWithValue("@KhoiLuong", khoiLuong);
            cmd.Parameters.AddWithValue("@DonGiaDeXuat", donGiaDeXuat);
            cmd.Parameters.AddWithValue("@MoTa", moTa ?? "");

            await conn.OpenAsync();

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetGuid(0); // Id của đơn mua vừa tạo
            }

            return null;
        }

        // ✏️ Cập nhật trạng thái (VD: “Đã thỏa thuận”, “Đã hủy”)
        public async Task<int> UpdateAsync(Guid id, string trangThai)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonMuaPheLieu_Update", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@TrangThai", trangThai);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }

        // ❌ Xóa đơn mua
        public async Task<int> DeleteAsync(Guid id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_DonMuaPheLieu_Delete", conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Id", id);

            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }
    }
}
