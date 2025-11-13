import React from "react";
import { Link } from "react-router-dom";
import "./Css/Home.css";

export default function HomePage() {
  const roles = [
    {
      title: "👤 Khách hàng",
      desc: "Đăng ký bán phế liệu, hẹn lịch thu gom và theo dõi quy trình minh bạch.",
      img: "https://marketingai.mediacdn.vn/wp-content/uploads/2023/02/chan-dung-khach-hang-1.jpg",
    },
    {
      title: "🚛 Nhân viên",
      desc: "Quản lý lịch hẹn, cập nhật vị trí và trạng thái thu gom theo thời gian thực.",
      img: "https://png.pngtree.com/png-vector/20231201/ourmid/pngtree-call-center-operator-icon-customer-png-image_10804574.png",
    },
    {
      title: "🏢 Doanh nghiệp",
      desc: "Quản lý đơn hàng, báo giá, theo dõi thống kê hiệu quả và tối ưu vận hành.",
      img: "https://vesinhcongnghiepbautroi.com/wp-content/uploads/2020/08/icon-thanh-lap-doanh-nghiep-SKY-e1656899857660.png",
    },
  ];

  return (
    <div className="home-container">
      {/* 🌿 HEADER MENU */}
      <header className="header">
        <div className="header-left">
          <img
            src="https://cdn-icons-png.flaticon.com/512/727/727399.png"
            alt="Logo"
            className="logo"
          />
          <div>
            <h1 className="app-title">Thu Mua Phế Liệu</h1>
            <p className="subtext">
              📞 0368 885 522 | 🏠 108 Miếu Bình Đông, Bình Hưng Hoà
            </p>
          </div>
        </div>

        <div className="header-right">
          <Link to="/login" className="btn login-btn">Đăng nhập</Link>
          <Link to="/register" className="btn register-btn">Đăng ký</Link>
        </div>
      </header>

      {/* 🌄 BANNER */}
      <section className="banner-section">
        <div className="banner-overlay">
          <img
            src="https://asiabizconsult.com/images/18905e4-business.png"
            alt="Banner"
            className="banner-img"
          />
        </div>
      </section>

      {/* 🧩 CHỌN VAI TRÒ */}
      <section className="roles-section">
        <h2 className="section-title">Lựa Chọn Vai Trò Của Bạn</h2>
        <div className="roles-container">
          {roles.map((role, index) => (
            <Link to="/login" key={index} className="role-card">
              <img src={role.img} alt={role.title} className="role-img" />
              <h3 className="role-title">{role.title}</h3>
              <p className="role-desc">{role.desc}</p>
            </Link>
          ))}
        </div>
      </section>

      {/* 🧠 GIỚI THIỆU */}
      <section className="about-section">
        <div className="about-content">
          <div className="about-text">
            <h2>Về Nền Tảng Thu Mua Phế Liệu</h2>
            <p>
              Hệ thống Thu Mua Phế Liệu giúp doanh nghiệp, cá nhân và nhân viên
              thu gom dễ dàng giao dịch, quản lý và tối ưu hoạt động.  
              Với công nghệ hiện đại, nền tảng mang đến quy trình minh bạch,
              nhanh chóng và thân thiện với môi trường 🌱
            </p>
            <Link to="/contact" className="btn-contact">
              Liên hệ ngay
            </Link>
          </div>
          <div className="about-img-box">
            <img
              src="https://img.freepik.com/free-vector/recycling-concept-illustration_114360-9389.jpg"
              alt="Giới thiệu"
            />
          </div>
        </div>
      </section>

      {/* 📞 FOOTER */}
      <footer className="footer">
        <div className="footer-grid">
          <div>
            <h3>📍 Địa chỉ</h3>
            <p>108 Miếu Bình Đông, Bình Hưng Hoà, TP. Hồ Chí Minh</p>
          </div>
          <div>
            <h3>📞 Liên hệ</h3>
            <p>0368 885 522</p>
            <p>support@pheliethuongmai.vn</p>
          </div>
          <div>
            <h3>🌐 Kết nối</h3>
            <p>Facebook | Zalo | Email</p>
          </div>
        </div>
        <p className="footer-bottom">
          © 2025 - Nền tảng Thu Mua Phế Liệu Việt Nam. All rights reserved.
        </p>
      </footer>
    </div>
  );
}
