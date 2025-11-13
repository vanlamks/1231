using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OTPResetPasswordController : ControllerBase
    {
        private readonly OTPResetPasswordService _service;

        public OTPResetPasswordController(OTPResetPasswordService service)
        {
            _service = service;
        }

        // 🔹 Lấy OTP của tài khoản
        [HttpGet("{taiKhoanId:guid}")]
        public async Task<IActionResult> GetByTaiKhoan(Guid taiKhoanId)
        {
            var otp = await _service.GetByTaiKhoanAsync(taiKhoanId);
            return otp == null ? NotFound() : Ok(otp);
        }

        // ➕ Thêm mã OTP
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OTPResetPasswordModel model)
        {
            var result = await _service.InsertAsync(model);
            return result > 0 ? Ok("✅ Mã OTP đã được tạo!") : BadRequest("❌ Tạo mã OTP thất bại!");
        }

        // ✏️ Cập nhật mã OTP đã sử dụng
        [HttpPut("mark-as-used")]
        public async Task<IActionResult> MarkAsUsed([FromBody] OTPResetPasswordModel model)
        {
            var result = await _service.MarkAsUsedAsync(model.TaiKhoanId, model.OTPCode);
            return result > 0 ? Ok("✅ Mã OTP đã được sử dụng!") : BadRequest("❌ Cập nhật thất bại!");
        }
    }
}
