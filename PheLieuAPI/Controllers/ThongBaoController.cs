using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThongBaoController : ControllerBase
    {
        private readonly ThongBaoService _service;

        public ThongBaoController(ThongBaoService service)
        {
            _service = service;
        }

        // 🟢 Lấy danh sách theo doanh nghiệp
        [HttpGet("DoanhNghiep/{doanhNghiepId:guid}")]
        public async Task<IActionResult> GetByDoanhNghiep(Guid doanhNghiepId)
        {
            var list = await _service.GetByDoanhNghiepAsync(doanhNghiepId);
            return Ok(list);
        }

        // 🟡 Tạo thông báo (sử dụng khi cần)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThongBaoCreateModel model)
        {
            var id = await _service.InsertAsync(model);
            return Ok(new { message = "📩 Tạo thông báo thành công!", id });
        }

        // 🟣 Đánh dấu đã xem
        [HttpPut("DaXem/{id:guid}")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var done = await _service.MarkAsReadAsync(id);

            return done > 0
                ? Ok(new { message = "Đã đánh dấu đã xem!" })
                : BadRequest(new { message = "Không tìm thấy thông báo!" });
        }

        // ❌ Xoá
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);

            return result > 0
                ? Ok(new { message = "🗑️ Xoá thành công!" })
                : BadRequest(new { message = "Xoá thất bại!" });
        }
    }
}
