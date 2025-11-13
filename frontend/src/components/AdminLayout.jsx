import { Outlet } from "react-router-dom";
import AdminSidebar from "./AdminSidebar";

// ⚙️ import đúng đường dẫn CSS
import "../pages/Css/AdminLayout.css";

export default function AdminLayout() {
  return (
    <div className="admin-layout">
      {/* Sidebar trái */}
      <aside className="sidebar">
        <AdminSidebar />
      </aside>

      {/* Nội dung bên phải */}
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
}
