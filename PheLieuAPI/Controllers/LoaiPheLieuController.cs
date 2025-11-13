using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoaiPheLieuController : ControllerBase
    {
        private readonly LoaiPheLieuService _service;

        public LoaiPheLieuController(LoaiPheLieuService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{maLoai}")]
        public async Task<IActionResult> GetById(string maLoai)
        {
            var item = await _service.GetByIdAsync(maLoai);
            return item == null ? NotFound("Không tìm thấy loại phế liệu!") : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LoaiPheLieuModel model)
        {
            var result = await _service.InsertAsync(model);
            return result > 0 ? Ok("✅ Thêm thành công!") : BadRequest("❌ Thêm thất bại!");
        }

        [HttpPut("{maLoai}")]
        public async Task<IActionResult> Update(string maLoai, [FromBody] LoaiPheLieuModel model)
        {
            model.MaLoai = maLoai;
            var result = await _service.UpdateAsync(model);
            return result > 0 ? Ok("✅ Cập nhật thành công!") : BadRequest("❌ Cập nhật thất bại!");
        }

        [HttpDelete("{maLoai}")]
        public async Task<IActionResult> Delete(string maLoai)
        {
            var result = await _service.DeleteAsync(maLoai);
            return result > 0 ? Ok("🗑️ Xóa thành công!") : BadRequest("❌ Xóa thất bại!");
        }
    }
}
