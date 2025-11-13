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
                        DonGiaDeXuat = Convert.ToDecimal(row["DonGiaDeXuat"]),
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
        public async Task<IActionResult> Insert([FromBody] DonMuaPheLieuModel model)
        {
            if (string.IsNullOrEmpty(model.TenPheLieu))
                return BadRequest("⚠️ Tên phế liệu không được để trống!");

            var result = await _service.InsertAsync(
                model.KhachHangId,
                model.DoanhNghiepId,
                model.TenPheLieu,
                model.KhoiLuong,
                model.DonGiaDeXuat,
                model.MoTa
            );

            return result > 0
                ? Ok(new { message = "✅ Đăng bài mua phế liệu thành công!" })
                : BadRequest("❌ Lỗi khi thêm đơn mua!");
        }

        // ✏️ Cập nhật trạng thái
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DonMuaPheLieuModel model)
        {
            var result = await _service.UpdateAsync(id, model.TrangThai ?? "Đang tìm nguồn cung");

            return result > 0
                ? Ok(new { message = "✅ Cập nhật trạng thái thành công!" })
                : BadRequest("❌ Cập nhật thất bại!");
        }

        // 🗑️ Xóa đơn mua
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result > 0
                ? Ok(new { message = "🗑️ Xóa đơn mua thành công!" })
                : BadRequest("❌ Xóa thất bại!");
        }
    }
}
