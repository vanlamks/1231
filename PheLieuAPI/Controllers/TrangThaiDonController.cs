using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrangThaiDonController : ControllerBase
    {
        private readonly TrangThaiDonService _service;

        public TrangThaiDonController(TrangThaiDonService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }
    }
}
