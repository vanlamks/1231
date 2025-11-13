using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrangThaiLichHenController : ControllerBase
    {
        private readonly TrangThaiLichHenService _service;

        public TrangThaiLichHenController(TrangThaiLichHenService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }
    }
}
