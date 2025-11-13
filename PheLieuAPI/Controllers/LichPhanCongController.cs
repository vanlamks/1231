using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;
using System.Data;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LichPhanCongController : ControllerBase
    {
        private readonly LichPhanCongService _service;

        public LichPhanCongController(LichPhanCongService service)
        {
            _service = service;
        }

        [HttpGet("doanhnghiep/{id}")]
        public async Task<IActionResult> GetByDoanhNghiep(Guid id)
        {
            var dt = await _service.GetByDoanhNghiepAsync(id);
            return Ok(dt);
        }

        [HttpGet("nhanvien/{id}")]
        public async Task<IActionResult> GetByNhanVien(Guid id)
        {
            var dt = await _service.GetByNhanVienAsync(id);
            return Ok(dt);
        }

        [HttpPost("auto")]
        public async Task<IActionResult> InsertAuto([FromBody] LichPhanCongModel model)
        {
            var result = await _service.InsertAutoAsync(model);
            return result > 0 ? Ok("✅ Tạo phân công thành công!") : BadRequest();
        }

        [HttpPut("nhan/{id}")]
        public async Task<IActionResult> Nhan(Guid id)
        {
            return (await _service.NhanAsync(id)) > 0 ? Ok("✅ Đã nhận việc!") : BadRequest();
        }

        [HttpPut("tuchoi/{id}")]
        public async Task<IActionResult> TuChoi(Guid id)
        {
            return (await _service.TuChoiAsync(id)) > 0 ? Ok("❌ Nhân viên từ chối!") : BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            return (await _service.DeleteAsync(id)) > 0 ? Ok("🗑️ Xóa lịch thành công!") : BadRequest();
        }
    }
}
