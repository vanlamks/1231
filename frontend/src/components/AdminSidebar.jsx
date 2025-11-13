import { NavLink } from "react-router-dom";
import "../pages/Css/AdminSidebar.css"; // import CSS đúng vị trí

export default function AdminSidebar() {
  const menuItems = [
    { name: "Tổng quan", path: "/dashboard/admin", icon: "🏠" },
    { name: "Phế liệu", path: "/dashboard/phelieu", icon: "♻️" },
    { name: "Loại phế liệu", path: "/dashboard/loaiphelieu", icon: "📦" },
    { name: "Lịch hẹn", path: "/dashboard/lichhen", icon: "🗓️" },
    { name: "Đơn thu gom", path: "/dashboard/donthugom", icon: "🚛" },
    { name: "Đơn mua phế liệu", path: "/dashboard/donmuaphelieu", icon: "💰" },
    { name: "Đơn bán phế liệu", path: "/dashboard/donbanphelieu", icon: "📊" },
    { name: "Vị trí người dùng", path: "/dashboard/vitringuoidung", icon: "📍" },
    { name: "Lịch sử vị trí NV", path: "/dashboard/lichsuvitrinhanvien", icon: "🧭" },
  ];

  return (
    <div className="sidebar-container">
      <div className="sidebar-header">
        <h2>♻️ Admin Panel</h2>
        <p>Quản lý hệ thống</p>
      </div>

      <nav className="sidebar-menu">
        {menuItems.map((item) => (
          <NavLink
            key={item.path}
            to={item.path}
            className={({ isActive }) =>
              isActive ? "menu-item active" : "menu-item"
            }
          >
            <span className="icon">{item.icon}</span>
            <span>{item.name}</span>
          </NavLink>
        ))}
      </nav>

      <div className="sidebar-footer">
        <button
          className="logout-btn"
          onClick={() => {
            localStorage.clear();
            window.location.href = "/";
          }}
        >
          🚪 Đăng xuất
        </button>
      </div>
    </div>
  );
}
