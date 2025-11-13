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

        // ✅ GET: Lấy danh sách khách hàng
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // ✅ POST: Thêm khách hàng
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] KhachHangModel model)
        {
            var result = await _service.InsertAsync(model);
            return result > 0 ? Ok("Thêm khách hàng thành công!") : BadRequest("Thêm thất bại!");
        }

        // ✅ PUT: Cập nhật
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] KhachHangModel model)
        {
            model.Id = id;
            var result = await _service.UpdateAsync(model);
            return result > 0 ? Ok("Cập nhật thành công!") : BadRequest("Cập nhật thất bại!");
        }

        // ✅ DELETE: Xóa mềm
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result > 0 ? Ok("Xóa (ngưng hoạt động) thành công!") : BadRequest("Xóa thất bại!");
        }
    }
}
