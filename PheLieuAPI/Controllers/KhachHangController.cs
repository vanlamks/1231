using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KhachHangController : ControllerBase
    {
        private readonly KhachHangService _service;

        public KhachHangController(KhachHangService service)
        {
            _service = service;
        }

        // Lấy tất cả khách hàng
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        // Lấy khách hàng theo ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var data = await _service.GetByIdAsync(id);
            return data == null ? NotFound() : Ok(data);
        }

        // Thêm khách hàng
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KhachHangModel model)
        {
            var result = await _service.InsertAsync(model);
            return result > 0 ? Ok("Thêm thành công") : BadRequest("Thêm thất bại");
        }

        // Cập nhật
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] KhachHangModel model)
        {
            model.Id = id;
            var result = await _service.UpdateAsync(model);
            return result > 0 ? Ok("Cập nhật thành công") : BadRequest("Cập nhật thất bại");
        }

        // Xóa mềm
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result > 0 ? Ok("Xóa thành công") : BadRequest("Xóa thất bại");
        }
    }
}
