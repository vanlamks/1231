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
  const [loaiList, setLoaiList] = useState([]);
  const [pheLieuList, setPheLieuList] = useState([]);
  const [selectedLoai, setSelectedLoai] = useState("");
  const [selectedPheLieu, setSelectedPheLieu] = useState("");
  const [selectedItem, setSelectedItem] = useState(null);
  const [moTa, setMoTa] = useState("");
  const [khoiLuong, setKhoiLuong] = useState("");
  const [giaDeXuat, setGiaDeXuat] = useState("");
  const [search, setSearch] = useState("");

  // 🧭 Lấy dữ liệu ban đầu
  useEffect(() => {
    axios.get(`${API}/DoanhNghiep`).then((res) => setDoanhNghiep(res.data[0] || {}));
    axios.get(`${API}/NhanVien`).then((res) => setNhanVienList(res.data));
    axios.get(`${API}/DonBanPheLieu`).then((res) => setLichSuBan(res.data));
    axios.get(`${API}/DonMuaPheLieu`).then((res) => setLichSuMua(res.data));
    axios.get(`${API}/LoaiPheLieu`).then((res) => setLoaiList(res.data));
    axios.get(`${API}/PheLieu`).then((res) => setPheLieuList(res.data));
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

  // 💰 Đăng bài bán phế liệu
  const handleDangBan = async () => {
    if (!selectedItem || !khoiLuong) return alert("⚠️ Nhập đầy đủ thông tin!");
    try {
      await axios.post(`${API}/DonBanPheLieu`, {
        DoanhNghiepId: doanhNghiep.id,
        TenPheLieu: selectedItem.tenPheLieu,
        KhoiLuong: parseFloat(khoiLuong),
        DonGia: selectedItem.donGia || 0,
        MoTa: moTa || selectedItem.moTa || "",
      });
      alert("🚀 Đăng bài bán phế liệu thành công!");
      setKhoiLuong("");
      setMoTa("");
    } catch (err) {
      alert("❌ Lỗi đăng bài bán: " + err.message);
    }
  };

  // 💸 Đăng bài mua phế liệu
  const handleDangMua = async () => {
    if (!selectedItem || !khoiLuong || !giaDeXuat)
      return alert("⚠️ Nhập đầy đủ thông tin!");
    try {
      await axios.post(`${API}/DonMuaPheLieu`, {
        DoanhNghiepId: doanhNghiep.id,
        TenPheLieu: selectedItem.tenPheLieu,
        KhoiLuong: parseFloat(khoiLuong),
        DonGiaDeXuat: parseFloat(giaDeXuat),
        MoTa: moTa || "",
      });
      alert("💰 Đăng bài mua phế liệu thành công!");
      setKhoiLuong("");
      setGiaDeXuat("");
      setMoTa("");
    } catch (err) {
      alert("❌ Lỗi đăng bài mua: " + err.message);
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

  // Lọc phế liệu
  const handleLoaiChange = (maLoai) => {
    setSelectedLoai(maLoai);
    setSelectedPheLieu("");
    setSelectedItem(null);
  };

  const handlePheLieuChange = (id) => {
    setSelectedPheLieu(id);
    const item = pheLieuList.find((p) => String(p.id) === String(id));
    setSelectedItem(item);
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
        <button onClick={() => setTab("ban")} className={tab === "ban" ? "active" : ""}>
          ♻️ Bán phế liệu
        </button>
        <button onClick={() => setTab("mua")} className={tab === "mua" ? "active" : ""}>
          💸 Mua phế liệu
        </button>
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

      {/* ====== THÔNG TIN DOANH NGHIỆP ====== */}
      {tab === "info" && (
        <div className="section-box">
          <h2>🏢 Thông tin doanh nghiệp</h2>
          <div className="form-grid">
            <label>Tên DN:</label>
            <input
              value={doanhNghiep.tenDoanhNghiep || ""}
              onChange={(e) =>
                setDoanhNghiep({ ...doanhNghiep, tenDoanhNghiep: e.target.value })
              }
            />
            <label>Mã số thuế:</label>
            <input
              value={doanhNghiep.maSoThueGiaiMa || ""}
              onChange={(e) =>
                setDoanhNghiep({ ...doanhNghiep, maSoThueGiaiMa: e.target.value })
              }
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
                    <button
                      onClick={() => handleDeleteNhanVien(nv.id)}
                      className="delete-btn"
                    >
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
          <h3>🟢 Bán phế liệu</h3>
          <table className="history-table">
            <thead>
              <tr>
                <th>Tên phế liệu</th>
                <th>Khối lượng</th>
                <th>Đơn giá</th>
                <th>Tổng tiền</th>
                <th>Mô tả</th>
                <th>Ngày đăng</th>
              </tr>
            </thead>
            <tbody>
              {lichSuBan.map((b, i) => (
                <tr key={i}>
                  <td>{b.tenPheLieu}</td>
                  <td>{b.khoiLuong} kg</td>
                  <td>{b.donGia?.toLocaleString()} đ</td>
                  <td>{(b.khoiLuong * b.donGia).toLocaleString()} đ</td>
                  <td>{b.moTa || "—"}</td>
                  <td>{b.createdAt ? new Date(b.createdAt).toLocaleString("vi-VN") : "—"}</td>
                </tr>
              ))}
            </tbody>
          </table>

          <h3>🔵 Mua phế liệu</h3>
          <table className="history-table">
            <thead>
              <tr>
                <th>Tên phế liệu</th>
                <th>Khối lượng</th>
                <th>Giá đề xuất</th>
                <th>Mô tả</th>
                <th>Ngày đăng</th>
              </tr>
            </thead>
            <tbody>
              {lichSuMua.map((m, i) => (
                <tr key={i}>
                  <td>{m.tenPheLieu}</td>
                  <td>{m.khoiLuong} kg</td>
                  <td>{m.donGiaDeXuat?.toLocaleString()} đ</td>
                  <td>{m.moTa || "—"}</td>
                  <td>{m.createdAt ? new Date(m.createdAt).toLocaleString("vi-VN") : "—"}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
