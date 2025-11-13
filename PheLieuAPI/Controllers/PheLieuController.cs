using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PheLieuController : ControllerBase
    {
        private readonly PheLieuService _service;

        public PheLieuController(PheLieuService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PheLieuModel model)
        {
            var result = await _service.InsertAsync(model);
            return result > 0 ? Ok("✅ Thêm phế liệu thành công!") : BadRequest("❌ Thêm thất bại!");
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PheLieuModel model)
        {
            model.Id = id;
            var result = await _service.UpdateAsync(model);
            return result > 0 ? Ok("✅ Cập nhật thành công!") : BadRequest("❌ Cập nhật thất bại!");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result > 0 ? Ok("🗑️ Xóa phế liệu thành công!") : BadRequest("❌ Xóa thất bại!");
        }
    }
}
