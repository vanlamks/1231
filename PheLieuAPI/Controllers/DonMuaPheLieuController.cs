using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Services;
using PheLieuAPI.Models;
using System.Data;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonMuaPheLieuController : ControllerBase
    {
        private readonly DonMuaPheLieuService _service;

        public DonMuaPheLieuController(DonMuaPheLieuService service)
        {
            _service = service;
        }

        // 🟢 Lấy tất cả đơn mua
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
                    DonGiaDeXuat = Convert.ToDecimal(row["DonGiaDeXuat"]),
                    MoTa = row["MoTa"]?.ToString(),
                    TrangThai = row["TrangThai"]?.ToString(),
                    CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                    NguoiDang = row["NguoiDang"]?.ToString(),
                    LoaiNguoiDang = row["LoaiNguoiDang"]?.ToString()
                });
            }

            return Ok(list);
        }

        // 🟢 Lấy theo khách hàng
        [HttpGet("KhachHang/{khachHangId:guid}")]
        public async Task<IActionResult> GetByKhachHang(Guid khachHangId)
        {
            var dt = await _service.GetAllAsync();
            var list = new List<object>();

            foreach (DataRow row in dt.Rows)
            {
                if (!row.IsNull("KhachHangId") &&
                    Guid.Parse(row["KhachHangId"].ToString()) == khachHangId)
                {
                    list.Add(new
                    {
                        Id = row["Id"],
                        TenPheLieu = row["TenPheLieu"],
                        KhoiLuong = row["KhoiLuong"],
                        DonGiaDeXuat = row["DonGiaDeXuat"],
                        MoTa = row["MoTa"],
                        TrangThai = row["TrangThai"],
                        CreatedAt = row["CreatedAt"]
                    });
                }
            }

            return Ok(list);
        }

        // ➕ Tạo đơn mua — trả về Id
        [HttpPost]
        public async Task<IActionResult> Insert([FromBody] DonMuaPheLieuModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TenPheLieu))
                return BadRequest("⚠️ Tên phế liệu không được trống!");

            var newId = await _service.InsertAsync(
                model.KhachHangId,
                model.DoanhNghiepId,
                model.TenPheLieu,
                model.KhoiLuong,
                model.DonGiaDeXuat,
                model.MoTa
            );

            if (newId == null)
                return BadRequest("❌ Lỗi khi tạo đơn mua!");

            return Ok(new
            {
                message = "✅ Đăng bài mua phế liệu thành công!",
                id = newId
            });
        }

        // ✏️ Cập nhật trạng thái
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DonMuaPheLieuModel model)
        {
            var result = await _service.UpdateAsync(id, model.TrangThai ?? "Đang tìm nguồn cung");

            return result > 0
                ? Ok(new { message = "✅ Cập nhật trạng thái thành công!" })
                : BadRequest("❌ Cập nhật thất bại!");
        }

        // ❌ Xóa đơn
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);

            return result > 0
                ? Ok(new { message = "🗑️ Xóa đơn mua phế liệu thành công!" })
                : BadRequest("❌ Xóa đơn thất bại!");
        }
    }
}
