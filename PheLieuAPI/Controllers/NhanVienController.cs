using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NhanVienController : ControllerBase
    {
        private readonly NhanVienService _service;

        public NhanVienController(NhanVienService service)
        {
            _service = service;
        }

        // 🔹 Lấy danh sách nhân viên
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // 🔹 Lấy theo ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var nv = await _service.GetByIdAsync(id);
            return nv == null ? NotFound() : Ok(nv);
        }

        // ➕ Thêm nhân viên
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NhanVienModel model)
        {
            var result = await _service.InsertAsync(model);
            return result > 0 ? Ok("✅ Thêm nhân viên thành công!") : BadRequest("❌ Thêm thất bại!");
        }

        // ✏️ Cập nhật
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] NhanVienModel model)
        {
            model.Id = id;
            var result = await _service.UpdateAsync(model);
            return result > 0 ? Ok("✅ Cập nhật nhân viên thành công!") : BadRequest("❌ Cập nhật thất bại!");
        }

        // ❌ Xóa mềm
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result > 0 ? Ok("🗑️ Xóa (ngưng hoạt động) thành công!") : BadRequest("❌ Xóa thất bại!");
        }
    }
}
