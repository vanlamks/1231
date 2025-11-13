using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DonThuGomController : ControllerBase
    {
        private readonly DonThuGomService _service;

        public DonThuGomController(DonThuGomService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DonThuGomModel model)
        {
            var result = await _service.InsertAsync(model);
            return result > 0 ? Ok("✅ Thêm đơn thu gom thành công!") : BadRequest("❌ Thêm thất bại!");
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] DonThuGomModel model)
        {
            model.Id = id;
            var result = await _service.UpdateAsync(model);
            return result > 0 ? Ok("✅ Cập nhật đơn thành công!") : BadRequest("❌ Cập nhật thất bại!");
        }

        [HttpPut("trangthai/{id:guid}")]
        public async Task<IActionResult> UpdateTrangThai(Guid id, [FromBody] DonThuGomModel model)
        {
            var result = await _service.UpdateTrangThaiAsync(id, model.TrangThaiCode, model.GhiChu);
            return result > 0 ? Ok("✅ Cập nhật trạng thái thành công!") : BadRequest("❌ Cập nhật thất bại!");
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result > 0 ? Ok("✅ Xóa đơn thành công!") : BadRequest("❌ Xóa thất bại!");
        }
    }
}
