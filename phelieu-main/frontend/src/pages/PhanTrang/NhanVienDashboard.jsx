import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

export default function NhanVienDashboard() {
  const navigate = useNavigate();

  useEffect(() => {
    const role = localStorage.getItem("role");
    if (role !== "NHAN_VIEN") {
      alert("Bạn không có quyền truy cập!");
      navigate("/");
    }
  }, [navigate]);

  return (
    <div className="min-h-screen bg-gradient-to-r from-green-100 via-emerald-50 to-lime-100 p-10">
      <div className="bg-white rounded-3xl shadow-lg p-10 max-w-4xl mx-auto text-center">
        <h1 className="text-4xl font-extrabold text-gray-700 mb-4">👷 Trang Nhân viên</h1>
        <p className="text-lg text-gray-600 mb-8">
          Xin chào <b>Nhân viên</b> — bạn có thể xem công việc, xác nhận đơn thu mua và cập nhật trạng thái làm việc.
        </p>
        <button
          onClick={() => navigate("/")}
          className="bg-red-500 text-white px-6 py-2 rounded-xl hover:bg-red-600 transition"
        >
          Đăng xuất
        </button>
      </div>
    </div>
  );
}
