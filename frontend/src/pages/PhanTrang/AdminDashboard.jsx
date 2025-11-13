import "../Css/AdminDashboard.css"; // import đúng đường dẫn

export default function AdminDashboard() {
  return (
    <div className="dashboard">
      <h1 className="dashboard-title">🏛️ Trang Quản Trị Hệ Thống</h1>
      <p className="dashboard-desc">
        Xin chào <b>Admin</b> — bạn có toàn quyền quản lý người dùng, dữ liệu và hệ thống.
      </p>

      <div className="card-container">
        <div className="card user">
          <h3>👥 Người dùng</h3>
          <p>Quản lý tài khoản, quyền truy cập và thông tin hệ thống.</p>
        </div>

        <div className="card waste">
          <h3>♻️ Phế liệu</h3>
          <p>Quản lý loại, giá và số lượng phế liệu trong hệ thống.</p>
        </div>

        <div className="card schedule">
          <h3>📅 Lịch hẹn</h3>
          <p>Theo dõi lịch hẹn, tiến độ và lịch sử thu gom.</p>
        </div>
      </div>
    </div>
  );
}
