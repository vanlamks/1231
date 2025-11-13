using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViTriNguoiDungController : ControllerBase
    {
        private readonly ViTriNguoiDungService _service;

        public ViTriNguoiDungController(ViTriNguoiDungService service)
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
        public async Task<IActionResult> Create([FromBody] ViTriNguoiDungModel model)
        {
            var result = await _service.InsertAsync(model);
            return result > 0 ? Ok("✅ Thêm vị trí thành công!") : BadRequest("❌ Thêm thất bại!");
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ViTriNguoiDungModel model)
        {
            var result = await _service.UpdateAsync(model);
            return result > 0 ? Ok("✅ Cập nhật vị trí thành công!") : BadRequest("❌ Cập nhật thất bại!");
        }

        [HttpDelete("{taiKhoanId:guid}")]
        public async Task<IActionResult> Delete(Guid taiKhoanId)
        {
            var result = await _service.DeleteAsync(taiKhoanId);
            return result > 0 ? Ok("✅ Xóa vị trí thành công!") : BadRequest("❌ Xóa thất bại!");
        }
    }
}
