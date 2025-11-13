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

        // 🧭 Lấy danh sách thông báo theo doanh nghiệp
        [HttpGet("DoanhNghiep/{doanhNghiepId:guid}")]
        public async Task<IActionResult> GetByDoanhNghiep(Guid doanhNghiepId)
        {
            var list = await _service.GetByDoanhNghiepAsync(doanhNghiepId);
            return Ok(list);
        }

        // ✅ Đánh dấu đã xem
        [HttpPut("DaXem/{id:guid}")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var result = await _service.MarkAsReadAsync(id);
            return result > 0
                ? Ok(new { message = "✅ Đã đánh dấu đã xem!" })
                : BadRequest(new { message = "❌ Không tìm thấy thông báo!" });
        }

        // ❌ Xóa thông báo
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result > 0
                ? Ok(new { message = "🗑️ Xóa thông báo thành công!" })
                : BadRequest(new { message = "❌ Xóa thất bại!" });
        }
    }
}
