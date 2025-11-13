import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import AdminLayout from "./components/AdminLayout";
import KhachHangLayout from "./pages/Css/KhachHangLayout"; // ✅ layout riêng
import HomePage from "./pages/HomePage";

// 🔑 Đăng nhập / đăng ký
import LoginPage from "./pages/DangNhap/LoginPage";
import RegisterPage from "./pages/DangNhap/RegisterPage";
import ForgotPasswordPage from "./pages/DangNhap/ForgotPasswordPage";


// 📊 Dashboard chính
import AdminDashboard from "./pages/PhanTrang/AdminDashboard";
import DoanhNghiepDashboard from "./pages/PhanTrang/DoanhNghiepDashboard";
import NhanVienDashboard from "./pages/PhanTrang/NhanVienDashboard";
import KhachHangDashboard from "./pages/PhanTrang/KhachHangDashboard";

// ♻️ Phế liệu
import PheLieuDashboard from "./pages/PheLieu/PheLieuDashboard";
import LoaiPheLieuDashboard from "./pages/PheLieu/LoaiPheLieuDashboard";
import LichHenDashboard from "./pages/PheLieu/LichHenDashboard";
import DonThuGomDashboard from "./pages/PheLieu/DonThuGomDashboard";

// 📍 Vị trí
import ViTriNguoiDungDashboard from "./pages/Vitri/ViTriNguoiDungDashboard";
import LichSuViTriNhanVienDashboard from "./pages/Vitri/LichSuViTriNhanVienDashboard";

// 💰 Quản lý đơn
import DonMuaPheLieuDashboard from "./pages/QuanLyDon/DonMuaPheLieuDashboard";
import DonBanPheLieuDashboard from "./pages/QuanLyDon/DonBanPheLieuDashboard";
import ThongBaoDoanhNghiepDashboard from './pages/QuanLyDon/ThongBaoDoanhNghiep';


function App() {
  return (
    <Router>
      <Routes>
        {/* 🌿 Trang chủ + đăng nhập */}
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/Forgot" element={<ForgotPasswordPage />} />
        

        {/* ================== 📊 DASHBOARD ADMIN, DN, NV ================== */}
        <Route path="/dashboard" element={<AdminLayout />}>
          {/* Vai trò */}
          <Route path="admin" element={<AdminDashboard />} />
          <Route path="doanhnghiep" element={<DoanhNghiepDashboard />} />
          <Route path="nhanvien" element={<NhanVienDashboard />} />

          {/* Phế liệu */}
          <Route path="phelieu" element={<PheLieuDashboard />} />
          <Route path="loaiphelieu" element={<LoaiPheLieuDashboard />} />
          <Route path="lichhen" element={<LichHenDashboard />} />
          <Route path="donthugom" element={<DonThuGomDashboard />} />

          {/* Vị trí */}
          <Route path="vitringuoidung" element={<ViTriNguoiDungDashboard />} />
          <Route path="lichsuvitrinhanvien" element={<LichSuViTriNhanVienDashboard />} />

          {/* Đơn */}
          <Route path="donmuaphelieu" element={<DonMuaPheLieuDashboard />} />
          <Route path="donbanphelieu" element={<DonBanPheLieuDashboard />} />
          <Route path="thongbao" element={<ThongBaoDoanhNghiepDashboard />} />
        </Route>

        {/* ================== 🧍 DASHBOARD KHÁCH HÀNG ================== */}
        <Route path="/dashboard/khachhang" element={<KhachHangLayout />}>
          <Route index element={<KhachHangDashboard />} />
        </Route>

        {/* Trang không tồn tại */}
        <Route path="*" element={<h1>404 - Không tìm thấy trang</h1>} />
      </Routes>
    </Router>
  );
}

export default App;
