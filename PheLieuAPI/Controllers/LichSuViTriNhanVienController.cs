using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LichSuViTriNhanVienController : ControllerBase
    {
        private readonly LichSuViTriNhanVienService _service;

        public LichSuViTriNhanVienController(LichSuViTriNhanVienService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{nhanVienId:guid}")]
        public async Task<IActionResult> GetByNhanVien(Guid nhanVienId)
        {
            var list = await _service.GetByNhanVienAsync(nhanVienId);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LichSuViTriNhanVienModel model)
        {
            var result = await _service.InsertAsync(model);
            return result > 0 ? Ok("✅ Ghi lại vị trí thành công!") : BadRequest("❌ Ghi thất bại!");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] LichSuViTriNhanVienModel model)
        {
            var result = await _service.UpdateAsync(model);
            return result > 0 ? Ok("✅ Cập nhật thành công!") : BadRequest("❌ Cập nhật thất bại!");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result > 0 ? Ok("✅ Xóa thành công!") : BadRequest("❌ Xóa thất bại!");
        }
    }
}
