using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThongBaoHeThongController : ControllerBase
    {
        private readonly ThongBaoHeThongService _service;

        public ThongBaoHeThongController(ThongBaoHeThongService service)
        {
            _service = service;
        }

        // 🔹 Lấy tất cả thông báo hệ thống
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // 🔹 Lấy thông báo hệ thống theo ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        // ➕ Thêm thông báo hệ thống
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ThongBaoHeThongModel model)
        {
            var result = await _service.InsertAsync(model);
            return result > 0 ? Ok("✅ Thông báo đã được tạo!") : BadRequest("❌ Tạo thông báo thất bại!");
        }

        // ✏️ Cập nhật trạng thái đã đọc
        [HttpPut("mark-as-read/{id:guid}")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var result = await _service.MarkAsReadAsync(id);
            return result > 0 ? Ok("✅ Đánh dấu thông báo đã đọc!") : BadRequest("❌ Cập nhật thất bại!");
        }
    }
}
