import { useState } from "react";
import axiosClient from "../../api/axiosClient";

export default function RegisterPage() {
  const [form, setForm] = useState({
    email: "",
    matKhau: "",
    soDienThoai: "",
    vaiTro: "KHACH_HANG",
  });
  const [message, setMessage] = useState("");

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleRegister = async (e) => {
    e.preventDefault();
    try {
      const res = await axiosClient.post("/taikhoan/register", form);
      setMessage(res.data.message);
    } catch (err) {
      setMessage(err.response?.data?.message || "Lỗi đăng ký!");
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-r from-green-100 to-yellow-100">
      <div className="bg-white p-8 rounded-2xl shadow-lg w-[400px]">
        <h2 className="text-2xl font-bold text-center mb-5 text-gray-700">
          🧾 Đăng ký tài khoản
        </h2>
        <form onSubmit={handleRegister}>
          <label className="block mb-2">Email</label>
          <input
            type="email"
            name="email"
            value={form.email}
            onChange={handleChange}
            required
            className="border w-full p-2 rounded mb-4"
            placeholder="Nhập email"
          />
          <label className="block mb-2">Mật khẩu</label>
          <input
            type="password"
            name="matKhau"
            value={form.matKhau}
            onChange={handleChange}
            required
            className="border w-full p-2 rounded mb-4"
            placeholder="Nhập mật khẩu"
          />
          <label className="block mb-2">Số điện thoại</label>
          <input
            type="text"
            name="soDienThoai"
            value={form.soDienThoai}
            onChange={handleChange}
            required
            className="border w-full p-2 rounded mb-4"
            placeholder="Nhập số điện thoại"
          />

          <label className="block mb-2">Vai trò</label>
          <select
            name="vaiTro"
            value={form.vaiTro}
            onChange={handleChange}
            className="border w-full p-2 rounded mb-4"
          >
            <option value="KHACH_HANG">Khách hàng</option>
            <option value="DOANH_NGHIEP">Doanh nghiệp</option>
            <option value="NHAN_VIEN">Nhân viên</option>
            <option value="ADMIN">Quản trị viên</option>
          </select>

          <button
            type="submit"
            className="bg-green-500 hover:bg-green-600 text-white py-2 rounded w-full transition"
          >
            Đăng ký
          </button>
        </form>
        {message && <p className="mt-4 text-center text-sm text-gray-700">{message}</p>}
        <p className="mt-4 text-center">
          Đã có tài khoản?{" "}
          <a href="/login" className="text-blue-600 hover:underline">
            Đăng nhập ngay
          </a>
        </p>
      </div>
    </div>
  );
}
