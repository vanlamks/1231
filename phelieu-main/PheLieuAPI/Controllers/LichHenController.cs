using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LichHenController : ControllerBase
    {
        private readonly LichHenService _service;

        public LichHenController(LichHenService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound("Không tìm thấy lịch hẹn!") : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LichHenModel model)
        {
            var result = await _service.InsertAsync(model);
            return result > 0 ? Ok("✅ Đã thêm lịch hẹn!") : BadRequest("❌ Thêm thất bại!");
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] LichHenModel model)
        {
            model.Id = id;
            var result = await _service.UpdateAsync(model);
            return result > 0 ? Ok("✅ Cập nhật thành công!") : BadRequest("❌ Cập nhật thất bại!");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result > 0 ? Ok("🗑️ Xóa thành công!") : BadRequest("❌ Xóa thất bại!");
        }
    }
}
