import React, { useEffect, useState } from "react";
import axios from "axios";
import "../Css/DoanhNghiepDashboard.css";

const API = "http://localhost:5071/api";

export default function DoanhNghiepDashboard() {
  const [tab, setTab] = useState("info");
  const [doanhNghiep, setDoanhNghiep] = useState({});
  const [nhanVienList, setNhanVienList] = useState([]);
  const [lichSuBan, setLichSuBan] = useState([]);
  const [lichSuMua, setLichSuMua] = useState([]);
  const [search, setSearch] = useState("");

  // 🧭 Lấy dữ liệu ban đầu
  useEffect(() => {
    axios.get(`${API}/DoanhNghiep`).then((res) => setDoanhNghiep(res.data[0] || {}));
    axios.get(`${API}/NhanVien`).then((res) => setNhanVienList(res.data));
    axios.get(`${API}/DonBanPheLieu`).then((res) => setLichSuBan(res.data));
    axios.get(`${API}/DonMuaPheLieu`).then((res) => setLichSuMua(res.data));
  }, []);

  // 🧩 Cập nhật thông tin doanh nghiệp
  const handleUpdate = async () => {
    try {
      await axios.put(`${API}/DoanhNghiep/${doanhNghiep.id}`, doanhNghiep);
      alert("✅ Cập nhật thông tin doanh nghiệp thành công!");
    } catch (err) {
      alert("❌ Lỗi cập nhật: " + err.message);
    }
  };

  // 🧑‍💼 Quản lý nhân viên
  const handleAddNhanVien = async () => {
    const hoTen = prompt("Nhập họ tên nhân viên mới:");
    if (!hoTen) return;
    try {
      await axios.post(`${API}/NhanVien`, {
        DoanhNghiepId: doanhNghiep.id,
        HoTen: hoTen,
      });
      alert("✅ Thêm nhân viên thành công!");
      const res = await axios.get(`${API}/NhanVien`);
      setNhanVienList(res.data);
    } catch (err) {
      alert("❌ Lỗi thêm nhân viên: " + err.message);
    }
  };

  const handleDeleteNhanVien = async (id) => {
    if (!window.confirm("Bạn có chắc muốn xóa nhân viên này?")) return;
    try {
      await axios.delete(`${API}/NhanVien/${id}`);
      alert("🗑️ Xóa nhân viên thành công!");
      setNhanVienList(nhanVienList.filter((nv) => nv.id !== id));
    } catch (err) {
      alert("❌ Lỗi xóa nhân viên: " + err.message);
    }
  };

  // 🔄 Tiếp nhận và từ chối đơn bán
  const handleXacNhanBan = async (id) => {
    try {
      await axios.put(`${API}/DonBanPheLieu/${id}`, {
        trangThai: "Đang thương lượng",
        moTa: "Doanh nghiệp đã tiếp nhận đơn",
      });
      alert("📦 Xác nhận đơn bán thành công!");
      const res = await axios.get(`${API}/DonBanPheLieu`);
      setLichSuBan(res.data);
    } catch (err) {
      alert("❌ Lỗi xác nhận!");
    }
  };

  const handleTuChoiBan = async (id) => {
    if (!window.confirm("Bạn chắc chắn muốn từ chối đơn này?")) return;
    try {
      await axios.delete(`${API}/DonBanPheLieu/${id}`);
      alert("🗑️ Đã từ chối đơn!");
      setLichSuBan(lichSuBan.filter((d) => d.id !== id));
    } catch {
      alert("❌ Lỗi từ chối!");
    }
  };

  // 🔄 Tiếp nhận và từ chối đơn mua
  const handleXacNhanMua = async (id) => {
    try {
      await axios.put(`${API}/DonMuaPheLieu/${id}`, {
        trangThai: "Đang thương lượng",
      });
      alert("🛒 Doanh nghiệp đã nhận đơn mua!");
      const res = await axios.get(`${API}/DonMuaPheLieu`);
      setLichSuMua(res.data);
    } catch {
      alert("❌ Lỗi nhận đơn!");
    }
  };

  const handleTuChoiMua = async (id) => {
    if (!window.confirm("Bạn chắc chắn muốn từ chối đơn này?")) return;
    try {
      await axios.delete(`${API}/DonMuaPheLieu/${id}`);
      alert("🗑️ Đã từ chối đơn!");
      setLichSuMua(lichSuMua.filter((d) => d.id !== id));
    } catch {
      alert("❌ Lỗi từ chối!");
    }
  };

  return (
    <div className="doanhnghiep-container">
      <div className="menu-tabs">
        <button onClick={() => setTab("info")} className={tab === "info" ? "active" : ""}>
          🏢 Thông tin DN
        </button>
        <button onClick={() => setTab("history")} className={tab === "history" ? "active" : ""}>
          📜 Lịch sử
        </button>
        <button onClick={() => setTab("nhanvien")} className={tab === "nhanvien" ? "active" : ""}>
          👩‍💼 Nhân viên
        </button>
        <button className="logout-btn" onClick={() => { localStorage.clear(); window.location.href = "/"; }}>
          🚪 Đăng xuất
        </button>
      </div>

      {/* ====== THÔNG TIN DOANH NGHIỆP ====== */}
      {tab === "info" && (
        <div className="section-box">
          <h2>🏢 Thông tin doanh nghiệp</h2>
          <div className="form-grid">
            <label>Tên DN:</label>
            <input
              value={doanhNghiep.tenDoanhNghiep || ""}
              onChange={(e) => setDoanhNghiep({ ...doanhNghiep, tenDoanhNghiep: e.target.value })}
            />
            <label>Mã số thuế:</label>
            <input
              value={doanhNghiep.maSoThueGiaiMa || ""}
              onChange={(e) => setDoanhNghiep({ ...doanhNghiep, maSoThueGiaiMa: e.target.value })}
            />
            <label>Địa chỉ:</label>
            <input
              value={doanhNghiep.diaChiText || ""}
              onChange={(e) => setDoanhNghiep({ ...doanhNghiep, diaChiText: e.target.value })}
            />
            <label>Website:</label>
            <input
              value={doanhNghiep.website || ""}
              onChange={(e) => setDoanhNghiep({ ...doanhNghiep, website: e.target.value })}
            />
            <label>Mô tả:</label>
            <textarea
              value={doanhNghiep.moTa || ""}
              onChange={(e) => setDoanhNghiep({ ...doanhNghiep, moTa: e.target.value })}
            />
          </div>
          <button onClick={handleUpdate} className="save-btn">
            💾 Cập nhật
          </button>
        </div>
      )}

      {/* ====== NHÂN VIÊN ====== */}
      {tab === "nhanvien" && (
        <div className="section-box">
          <h2>👩‍💼 Quản lý nhân viên</h2>
          <button onClick={handleAddNhanVien} className="add-btn">➕ Thêm nhân viên</button>
          <table className="history-table">
            <thead>
              <tr>
                <th>Họ tên</th>
                <th>Email</th>
                <th>Trạng thái</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {nhanVienList.map((nv, i) => (
                <tr key={i}>
                  <td>{nv.hoTen}</td>
                  <td>{nv.email}</td>
                  <td>{nv.trangThaiSanSang ? "✅ Sẵn sàng" : "🕓 Bận"}</td>
                  <td>
                    <button onClick={() => handleDeleteNhanVien(nv.id)} className="delete-btn">
                      🗑️ Xóa
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* ====== LỊCH SỬ GIAO DỊCH ====== */}
      {tab === "history" && (
        <div className="section-box">
          <h2>📜 Lịch sử giao dịch</h2>
          
          {/* Đơn bán phế liệu */}
          <h3>🟢 Đơn bán phế liệu</h3>
          <table className="history-table">
            <thead>
              <tr>
                <th>Khách hàng</th>
                <th>Phế liệu</th>
                <th>Khối lượng</th>
                <th>Đơn giá</th>
                <th>Trạng thái</th>
                <th>Thời gian</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {lichSuBan.map((d) => (
                <tr key={d.id}>
                  <td>{d.nguoiDang}</td>
                  <td>{d.tenPheLieu}</td>
                  <td>{d.khoiLuong} kg</td>
                  <td>{d.donGia?.toLocaleString()} đ</td>
                  <td>{d.trangThai}</td>
                  <td>{new Date(d.createdAt).toLocaleString()}</td>
                  <td>
                    {d.trangThai === "Chờ giao dịch" && (
                      <>
                        <button className="ok-btn" onClick={() => handleXacNhanBan(d.id)}>
                          ✔ Tiếp nhận
                        </button>
                        <button className="delete-btn" onClick={() => handleTuChoiBan(d.id)}>
                          ❌ Từ chối
                        </button>
                      </>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>

          {/* Đơn mua phế liệu */}
          <h3>🔵 Đơn mua phế liệu</h3>
          <table className="history-table">
            <thead>
              <tr>
                <th>Khách hàng</th>
                <th>Phế liệu</th>
                <th>Khối lượng</th>
                <th>Giá đề xuất</th>
                <th>Trạng thái</th>
                <th>Thời gian</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {lichSuMua.map((d) => (
                <tr key={d.id}>
                  <td>{d.nguoiDang}</td>
                  <td>{d.tenPheLieu}</td>
                  <td>{d.khoiLuong} kg</td>
                  <td>{d.donGiaDeXuat?.toLocaleString()} đ</td>
                  <td>{d.trangThai}</td>
                  <td>{new Date(d.createdAt).toLocaleString()}</td>
                  <td>
                    {d.trangThai === "Chờ giao dịch" && (
                      <>
                        <button className="ok-btn" onClick={() => handleXacNhanMua(d.id)}>
                          ✔ Tiếp nhận
                        </button>
                        <button className="delete-btn" onClick={() => handleTuChoiMua(d.id)}>
                          ❌ Từ chối
                        </button>
                      </>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
