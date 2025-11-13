import React from "react";
import { Outlet, useNavigate } from "react-router-dom";

function KhachHangLayout() {
  const navigate = useNavigate();

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-100 via-white to-pink-100">
      {/* Thanh điều hướng */}
      <nav className="flex justify-between items-center bg-white shadow px-8 py-4">
        <h1
          onClick={() => navigate("/dashboard/khachhang")}
          className="text-2xl font-bold text-blue-600 cursor-pointer"
        >
          ♻️ PheLieuApp
        </h1>
        <button
          onClick={() => {
            localStorage.clear();
            navigate("/");
          }}
          className="bg-red-500 text-white px-4 py-2 rounded-lg hover:bg-red-600"
        >
          Đăng xuất
        </button>
      </nav>

      {/* Nội dung chính */}
      <main className="p-10">
        <Outlet />
      </main>
    </div>
  );
}

export default KhachHangLayout; // ✅ BẮT BUỘC PHẢI CÓ
