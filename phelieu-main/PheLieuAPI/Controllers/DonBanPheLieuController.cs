using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Services;
using PheLieuAPI.Models;
using System.Data;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonBanPheLieuController : ControllerBase
    {
        private readonly DonBanPheLieuService _service;

        public DonBanPheLieuController(DonBanPheLieuService service)
        {
            _service = service;
        }

        // 🟢 Lấy tất cả đơn
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dt = await _service.GetAllAsync();
            var list = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new
                {
                    Id = row["Id"],
                    KhachHangId = row["KhachHangId"],
                    DoanhNghiepId = row["DoanhNghiepId"],
                    TenPheLieu = row["TenPheLieu"]?.ToString(),
                    KhoiLuong = Convert.ToDecimal(row["KhoiLuong"]),
                    DonGia = Convert.ToDecimal(row["DonGia"]),
                    MoTa = row["MoTa"]?.ToString(),
                    TrangThai = row["TrangThai"]?.ToString(),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                    NguoiDang = row["NguoiDang"]?.ToString(),
                    LoaiNguoiDang = row["LoaiNguoiDang"]?.ToString()
                });
            }

            return Ok(list);
        }

        // 🔍 Lấy theo Id khách hàng
        [HttpGet("KhachHang/{khachHangId:guid}")]
        public async Task<IActionResult> GetByKhachHang(Guid khachHangId)
        {
            var dt = await _service.GetAllAsync();
            var list = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                if (row.Table.Columns.Contains("KhachHangId")
                    && row["KhachHangId"] != DBNull.Value
                    && Guid.Parse(row["KhachHangId"].ToString()) == khachHangId)
                {
                    list.Add(new
                    {
                        Id = row["Id"],
                        TenPheLieu = row["TenPheLieu"]?.ToString(),
                        KhoiLuong = Convert.ToDecimal(row["KhoiLuong"]),
                        DonGia = Convert.ToDecimal(row["DonGia"]),
                        MoTa = row["MoTa"]?.ToString(),
                        TrangThai = row["TrangThai"]?.ToString(),
                        CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                        NguoiDang = row["NguoiDang"]?.ToString(),
                        LoaiNguoiDang = row["LoaiNguoiDang"]?.ToString()
                    });
                }
            }

            return Ok(list);
        }

        // ➕ Thêm mới
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] DonBanPheLieuModel model)
        {
            if (string.IsNullOrEmpty(model.TenPheLieu))
                return BadRequest("⚠️ Tên phế liệu không được để trống!");

            var result = await _service.InsertAsync(
                model.KhachHangId,
                model.DoanhNghiepId,
                model.TenPheLieu,
                model.KhoiLuong,
                model.DonGia,
                model.MoTa
            );

            return result > 0
                ? Ok(new { message = "✅ Đăng bài bán phế liệu thành công!" })
                : BadRequest("❌ Lỗi khi thêm đơn bán!");
        }

        // ✏️ Cập nhật trạng thái và chi tiết
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DonBanPheLieuModel model)
        {
            var result = await _service.UpdateAsync(
                id,
                model.KhoiLuong,
                model.DonGia,
                model.MoTa,
                model.TrangThai ?? "Chờ giao dịch"
            );

            return result > 0
                ? Ok(new { message = "✅ Cập nhật đơn bán thành công!" })
                : BadRequest("❌ Cập nhật thất bại!");
        }

        // 🗑️ Xóa đơn
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result > 0
                ? Ok(new { message = "🗑️ Xóa đơn bán thành công!" })
                : BadRequest("❌ Xóa thất bại!");
        }
    }
}
