using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminUserController : ControllerBase
    {
        private readonly AdminUserService _service;

        public AdminUserController(AdminUserService service)
        {
            _service = service;
        }

        // 🔹 Lấy tất cả admin
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
            var item = await _service.GetByIdAsync(id);
            if (item == null)
                return NotFound(new { message = "Không tìm thấy admin!" });
            return Ok(item);
        }

        // ➕ Thêm admin
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminUserModel model)
        {
            var result = await _service.InsertAsync(model);
            return result > 0 ? Ok(new { message = "Thêm admin thành công!" }) 
                              : BadRequest(new { message = "Thêm thất bại!" });
        }

        // ✏️ Cập nhật admin
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AdminUserModel model)
        {
            model.Id = id;
            var result = await _service.UpdateAsync(model);
            return result > 0 ? Ok(new { message = "Cập nhật thành công!" })
                              : BadRequest(new { message = "Cập nhật thất bại!" });
        }

        // ❌ Xóa mềm
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result > 0 ? Ok(new { message = "Xóa (ngưng hoạt động) thành công!" })
                              : BadRequest(new { message = "Xóa thất bại!" });
        }
    }
}
