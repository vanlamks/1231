using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoanhNghiepController : ControllerBase
    {
        private readonly DoanhNghiepService _service;

        public DoanhNghiepController(DoanhNghiepService service)
        {
            _service = service;
        }

        // 🔹 GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // 🔹 GET BY ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        // ➕ POST (Thêm mới)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DoanhNghiepModel model)
        {
            var result = await _service.InsertAsync(model);
            return result > 0 ? Ok("Thêm doanh nghiệp thành công!") : BadRequest("Thêm thất bại!");
        }

        // ✏️ PUT (Cập nhật)
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DoanhNghiepModel model)
        {
            model.Id = id;
            var result = await _service.UpdateAsync(model);
            return result > 0 ? Ok("Cập nhật thành công!") : BadRequest("Cập nhật thất bại!");
        }

        // ❌ DELETE (Xóa mềm)
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result > 0 ? Ok("Xóa (ngưng hoạt động) thành công!") : BadRequest("Xóa thất bại!");
        }
    }
}
