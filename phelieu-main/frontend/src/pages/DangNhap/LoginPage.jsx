import { useState } from "react";
import { FaEnvelope, FaLock } from "react-icons/fa";
import axiosClient from "../../api/axiosClient";

export default function LoginPage() {
  const [form, setForm] = useState({ taiKhoan: "", matKhau: "" });
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleLogin = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage("");
    try {
      const res = await axiosClient.post("/taikhoan/login", form);

      const role = res.data.role?.toUpperCase();
      const userId = res.data.userId;

      if (!role) throw new Error("Không xác định được vai trò người dùng!");

      // ✅ Lưu thông tin tài khoản chung
      localStorage.setItem("role", role);
      localStorage.setItem("taiKhoanId", userId);
      localStorage.setItem("email", res.data.email || "");
      localStorage.setItem("soDienThoai", res.data.soDienThoai || "");

      // ✅ Lưu theo từng vai trò
     if (res.data.thongTinDoanhNghiep) {
  localStorage.setItem("doanhnghiep_id", res.data.thongTinDoanhNghiep.id);
}
if (res.data.thongTinNhanVien) {
  localStorage.setItem("nhanvien_id", res.data.thongTinNhanVien.id);
}
if (res.data.thongTinKhachHang) {
  localStorage.setItem("khachhang_id", res.data.thongTinKhachHang.id);
}

      // ✅ Điều hướng theo role
      switch (role) {
        case "ADMIN":
          window.location.href = "/dashboard/admin";
          break;
        case "DOANH_NGHIEP":
          window.location.href = "/dashboard/doanhnghiep";
          break;
        case "NHAN_VIEN":
          window.location.href = "/dashboard/nhanvien";
          break;
        case "KHACH_HANG":
          window.location.href = "/dashboard/khachhang";
          break;
        default:
          setMessage("❌ Vai trò không hợp lệ!");
      }

      setMessage(res.data.message || "✅ Đăng nhập thành công!");
    } catch (err) {
      setMessage(err.response?.data?.message || "❌ Lỗi đăng nhập!");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex font-[Poppins] bg-gradient-to-br from-blue-100 via-green-50 to-teal-100">
      
      {/* Cột trái */}
      <div className="hidden lg:flex w-1/2 bg-gradient-to-br from-green-400 via-teal-500 to-blue-600 text-white items-center justify-center flex-col p-10 shadow-2xl">
        <img
          src="https://cdn-icons-png.flaticon.com/512/6151/6151925.png"
          alt="Phế liệu banner"
          className="w-80 h-80 mb-6 animate-float drop-shadow-xl"
        />
        <h1 className="text-4xl font-extrabold mb-3 text-center drop-shadow-lg">
          ♻️ Hệ thống Quản lý Phế liệu
        </h1>
        <p className="text-lg text-center opacity-90 max-w-md leading-relaxed">
          Giải pháp số hóa thu mua, tái chế và quản lý phế liệu toàn diện
          dành cho <strong>Doanh nghiệp</strong>, <strong>Nhân viên</strong> và{" "}
          <strong>Khách hàng</strong>.
        </p>
      </div>

      {/* Cột phải */}
      <div className="flex flex-col justify-center items-center w-full lg:w-1/2 p-8">
        <div className="bg-white/90 backdrop-blur-md p-10 rounded-3xl shadow-xl w-full max-w-md transition hover:shadow-green-200">
          <h2 className="text-3xl font-bold text-center mb-8 text-gray-700">
            🔐 Đăng nhập hệ thống
          </h2>

          <form onSubmit={handleLogin} className="space-y-5">
            
            {/* Tài khoản */}
            <div className="relative">
              <FaEnvelope className="absolute left-3 top-3.5 text-gray-400" />
              <input
                type="text"
                name="taiKhoan"
                value={form.taiKhoan}
                onChange={handleChange}
                required
                className="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-400 outline-none transition"
                placeholder="Nhập email hoặc số điện thoại"
              />
            </div>

            {/* Mật khẩu */}
            <div className="relative">
              <FaLock className="absolute left-3 top-3.5 text-gray-400" />
              <input
                type="password"
                name="matKhau"
                value={form.matKhau}
                onChange={handleChange}
                required
                className="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-400 outline-none transition"
                placeholder="Nhập mật khẩu"
              />
            </div>

            {/* Nút đăng nhập */}
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-gradient-to-r from-green-500 to-teal-500 text-white font-semibold py-2.5 rounded-lg shadow-md hover:shadow-lg hover:opacity-90 transition-all duration-300"
            >
              {loading ? "⏳ Đang đăng nhập..." : "Đăng nhập"}
            </button>
          </form>

          {/* Thông báo */}
          {message && (
            <p
              className={`mt-4 text-center text-sm font-medium ${
                message.includes("thành công")
                  ? "text-green-600"
                  : "text-red-500"
              }`}
            >
              {message}
            </p>
          )}

          {/* Link đăng ký */}
          <p className="mt-6 text-center text-gray-600">
            Chưa có tài khoản?{" "}
            <a
              href="/register"
              className="text-green-600 font-semibold hover:underline"
            >
              Đăng ký ngay
            </a>
          </p>
        </div>
      </div>
    </div>
  );
}
