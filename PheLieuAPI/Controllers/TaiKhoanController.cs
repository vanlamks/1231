using Microsoft.AspNetCore.Mvc;
using PheLieuAPI.Models;
using PheLieuAPI.Services;

namespace PheLieuAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaiKhoanController : ControllerBase
    {
        private readonly TaiKhoanService _service;

        public TaiKhoanController(TaiKhoanService service)
        {
            _service = service;
        }

        // 🔹 Lấy danh sách tài khoản
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // 🔹 Lấy theo ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        // ➕ Thêm tài khoản
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TaiKhoanModel model)
        {
            var result = await _service.InsertAsync(model);
            return result > 0 ? Ok("✅ Thêm thành công!") : BadRequest("❌ Thêm thất bại!");
        }

        // ✏️ Cập nhật
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TaiKhoanModel model)
        {
            model.Id = id;
            var result = await _service.UpdateAsync(model);
            return result > 0 ? Ok("✅ Cập nhật thành công!") : BadRequest("❌ Cập nhật thất bại!");
        }

        // ❌ Xóa mềm
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);
            return result > 0 ? Ok("🗑️ Đã ngưng hoạt động tài khoản!") : BadRequest("❌ Xóa thất bại!");
        }

        // 🔐 Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DangNhapModel model)
        {
            if (string.IsNullOrEmpty(model.TaiKhoan) || string.IsNullOrEmpty(model.MatKhau))
                return BadRequest(new { message = "⚠️ Thiếu thông tin đăng nhập!" });

            var user = await _service.LoginAsync(model);
            if (user == null)
                return Unauthorized(new { message = "❌ Sai tài khoản hoặc mật khẩu!" });

            return Ok(new
{
    message = "Đăng nhập thành công!",
    role = user.VaiTro,
    userId = user.Id,
    email = user.Email,
    soDienThoai = user.SoDienThoai,
    thongTinKhachHang = user.ThongTinKhachHang,
    thongTinDoanhNghiep = user.ThongTinDoanhNghiep,
    thongTinNhanVien = user.ThongTinNhanVien,
    thongTinAdmin = user.ThongTinAdmin
});

        }

        // 📝 Đăng ký
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DangKyModel model)
        {
            try
            {
                // Validate cơ bản
                if (string.IsNullOrEmpty(model.Email) ||
                    string.IsNullOrEmpty(model.MatKhau) ||
                    string.IsNullOrEmpty(model.SoDienThoai))
                {
                    return BadRequest(new { message = "⚠️ Thiếu thông tin bắt buộc!" });
                }

                // Gọi service thực thi
                var newId = await _service.RegisterAsync(model);

                if (newId == null)
                    return BadRequest(new { message = "❌ Đăng ký thất bại hoặc tài khoản đã tồn tại!" });

                return Ok(new
                {
                    message = "✅ Đăng ký thành công!",
                    TaiKhoanId = newId,
                    VaiTro = model.VaiTro
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "❌ Lỗi hệ thống: " + ex.Message });
            }
        }
    }
}
