import { useState } from "react";
import { FaEnvelope } from "react-icons/fa";
import axiosClient from "../../api/axiosClient";

export default function ForgotPasswordPage() {
  const [taiKhoan, setTaiKhoan] = useState(""); // Email hoặc Số điện thoại
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);
  const [otp, setOtp] = useState("");
  const [newPassword, setNewPassword] = useState("");

  // Xử lý thay đổi giá trị trong form
  const handleChange = (e) => {
    const { name, value } = e.target;
    if (name === "taiKhoan") {
      setTaiKhoan(value);
    } else if (name === "otp") {
      setOtp(value);
    } else if (name === "newPassword") {
      setNewPassword(value);
    }
  };

  // Gửi yêu cầu OTP
  const handleSendOtp = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage("");
    try {
      const res = await axiosClient.post("/taikhoan/forgot-password", { taiKhoan });
      setMessage("✅ Mã OTP đã được gửi!");
    } catch (err) {
      setMessage("❌ Không tìm thấy tài khoản!");
    } finally {
      setLoading(false);
    }
  };

  // Đặt lại mật khẩu
  const handleResetPassword = async (e) => {
    e.preventDefault();
    setLoading(true);
    setMessage("");
    try {
      const res = await axiosClient.post("/taikhoan/reset-password", {
        taiKhoan,
        otp,
        matKhauMoi: newPassword,
      });
      setMessage("✅ Mật khẩu đã được thay đổi!");
    } catch (err) {
      setMessage("❌ Mã OTP không hợp lệ hoặc đã hết hạn!");
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
            🔐 Quên mật khẩu
          </h2>

          {/* Gửi OTP */}
          <form onSubmit={handleSendOtp} className="space-y-5">
            <div className="relative">
              <FaEnvelope className="absolute left-3 top-3.5 text-gray-400" />
              <input
                type="text"
                name="taiKhoan"
                value={taiKhoan}
                onChange={handleChange}
                required
                className="w-full pl-10 pr-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-400 outline-none transition"
                placeholder="Nhập email hoặc số điện thoại"
              />
            </div>
            <button
              type="submit"
              disabled={loading}
              className="w-full bg-gradient-to-r from-green-500 to-teal-500 text-white font-semibold py-2.5 rounded-lg shadow-md hover:shadow-lg hover:opacity-90 transition-all duration-300"
            >
              {loading ? "⏳ Đang gửi OTP..." : "Gửi mã OTP"}
            </button>
          </form>

          {/* Hiển thị mã OTP và thay đổi mật khẩu */}
          {message && (
            <p
              className={`mt-4 text-center text-sm font-medium ${
                message.includes("thành công") ? "text-green-600" : "text-red-500"
              }`}
            >
              {message}
            </p>
          )}

          {message.includes("Mã OTP") && (
            <form onSubmit={handleResetPassword} className="space-y-5 mt-5">
              {/* Nhập OTP */}
              <div className="relative">
                <input
                  type="text"
                  name="otp"
                  value={otp}
                  onChange={handleChange}
                  required
                  className="w-full pl-3 pr-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-400 outline-none transition"
                  placeholder="Nhập mã OTP"
                />
              </div>

              {/* Nhập mật khẩu mới */}
              <div className="relative">
                <input
                  type="password"
                  name="newPassword"
                  value={newPassword}
                  onChange={handleChange}
                  required
                  className="w-full pl-3 pr-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-green-400 outline-none transition"
                  placeholder="Nhập mật khẩu mới"
                />
              </div>

              <button
                type="submit"
                disabled={loading}
                className="w-full bg-gradient-to-r from-green-500 to-teal-500 text-white font-semibold py-2.5 rounded-lg shadow-md hover:shadow-lg hover:opacity-90 transition-all duration-300"
              >
                {loading ? "⏳ Đang thay đổi mật khẩu..." : "Đặt lại mật khẩu"}
              </button>
            </form>
          )}

          {/* Link đăng nhập */}
          <p className="mt-6 text-center text-gray-600">
            Đã nhớ mật khẩu?{" "}
            <a
              href="/login"
              className="text-green-600 font-semibold hover:underline"
            >
              Đăng nhập ngay
            </a>
          </p>
        </div>
      </div>
    </div>
  );
}
