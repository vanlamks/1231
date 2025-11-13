import React, { useEffect, useState } from "react";
import axios from "axios";
import "../Css/KhachHangDashboard.css";

const API = "http://localhost:5071/api";

export default function KhachHangDashboard() {
  const [tab, setTab] = useState("info");
  const [khachHang, setKhachHang] = useState({});
  const [lichSuBan, setLichSuBan] = useState([]);
  const [lichSuMua, setLichSuMua] = useState([]);
  const [doanhNghiep, setDoanhNghiep] = useState([]);
  const [search, setSearch] = useState("");
  const [loaiList, setLoaiList] = useState([]);
  const [pheLieuList, setPheLieuList] = useState([]);
  const [selectedLoai, setSelectedLoai] = useState("");
  const [selectedPheLieu, setSelectedPheLieu] = useState("");
  const [selectedItem, setSelectedItem] = useState(null);
  const [moTa, setMoTa] = useState("");
  const [khoiLuong, setKhoiLuong] = useState("");
  const [giaDeXuat, setGiaDeXuat] = useState("");

  // 🧭 Lấy dữ liệu ban đầu
  useEffect(() => {
    const khachHangId = localStorage.getItem("khachhang_id");
    if (!khachHangId) {
      alert("Không tìm thấy thông tin khách hàng. Vui lòng đăng nhập lại!");
      window.location.href = "/";
      return;
    }

    // ✅ Lấy thông tin khách hàng theo ID
    axios
      .get(`${API}/KhachHang/${khachHangId}`)
      .then((res) => setKhachHang(res.data))
      .catch((err) => console.error("Lỗi lấy thông tin khách hàng:", err));

    // ✅ Lịch sử bán/mua riêng của khách hàng đó
    axios
      .get(`${API}/DonBanPheLieu/KhachHang/${khachHangId}`)
      .then((res) => setLichSuBan(res.data))
      .catch(() => setLichSuBan([]));

    axios
      .get(`${API}/DonMuaPheLieu/KhachHang/${khachHangId}`)
      .then((res) => setLichSuMua(res.data))
      .catch(() => setLichSuMua([]));

    // ✅ Các dữ liệu phụ
    axios.get(`${API}/DoanhNghiep`).then((res) => setDoanhNghiep(res.data));
    axios.get(`${API}/LoaiPheLieu`).then((res) => setLoaiList(res.data));
    axios.get(`${API}/PheLieu`).then((res) => setPheLieuList(res.data));
  }, []);

  // 🔄 Cập nhật thông tin khách hàng
  const handleUpdate = async () => {
    try {
      await axios.put(`${API}/KhachHang/${khachHang.id || khachHang.Id}`, khachHang);
      alert("✅ Cập nhật thông tin thành công!");
    } catch (err) {
      alert("❌ Lỗi cập nhật: " + err.message);
    }
  };

  // 🔎 Lọc doanh nghiệp
  const filteredDoanhNghiep = doanhNghiep.filter((d) =>
    d.tenDoanhNghiep?.toLowerCase().includes(search.toLowerCase())
  );

  // 🟢 Chọn loại & phế liệu
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

  // ♻️ Đăng bài bán phế liệu + gửi thông báo
  const handleDangBan = async () => {
    if (!selectedItem || !khoiLuong) return alert("⚠️ Nhập đầy đủ thông tin!");
    try {
      await axios.post(`${API}/DonBanPheLieu`, {
        KhachHangId: khachHang.id || khachHang.Id,
        TenPheLieu: selectedItem.tenPheLieu,
        KhoiLuong: parseFloat(khoiLuong),
        DonGia: selectedItem.donGia || 0,
        MoTa: moTa || selectedItem.moTa || "",
      });

      await axios.post(`${API}/ThongBao`, {
        Loai: "DonBanMoi",
        NoiDung: `Khách hàng ${khachHang.hoTen} vừa đăng bán ${selectedItem.tenPheLieu} (${khoiLuong} kg)`,
        TenPheLieu: selectedItem.tenPheLieu,
      });

      alert("🚀 Đăng bài bán phế liệu thành công!");
      setKhoiLuong("");
      setMoTa("");
    } catch (err) {
      alert("❌ Lỗi đăng bài bán: " + err.message);
    }
  };

  // 💸 Đăng bài mua phế liệu + gửi thông báo
  const handleDangMua = async () => {
    if (!selectedItem || !khoiLuong || !giaDeXuat)
      return alert("⚠️ Nhập đầy đủ thông tin!");
    try {
      await axios.post(`${API}/DonMuaPheLieu`, {
        KhachHangId: khachHang.id || khachHang.Id,
        TenPheLieu: selectedItem.tenPheLieu,
        KhoiLuong: parseFloat(khoiLuong),
        DonGiaDeXuat: parseFloat(giaDeXuat),
        MoTa: moTa || "",
      });

      await axios.post(`${API}/ThongBao`, {
        Loai: "DonMuaMoi",
        NoiDung: `Khách hàng ${khachHang.hoTen} vừa đăng mua ${selectedItem.tenPheLieu} (${khoiLuong} kg, giá đề xuất ${giaDeXuat} đ/kg)`,
        TenPheLieu: selectedItem.tenPheLieu,
      });

      alert("💰 Đăng bài mua phế liệu thành công!");
      setKhoiLuong("");
      setGiaDeXuat("");
      setMoTa("");
    } catch (err) {
      alert("❌ Lỗi đăng bài mua: " + err.message);
    }
  };

  return (
    <div className="khachhang-container">
      {/* ================== MENU TAB ================== */}
      <div className="menu-tabs">
        <button onClick={() => setTab("info")} className={tab === "info" ? "active" : ""}>
          👤 Thông tin cá nhân
        </button>
        <button onClick={() => setTab("history")} className={tab === "history" ? "active" : ""}>
          🕓 Lịch sử
        </button>
        <button onClick={() => setTab("search")} className={tab === "search" ? "active" : ""}>
          🏢 Doanh nghiệp
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

      {/* ================== THÔNG TIN CÁ NHÂN ================== */}
      {tab === "info" && (
        <div className="section-box">
          <h2>🧍 Thông tin khách hàng</h2>
          <div className="form-grid">
            <label>Họ và tên:</label>
            <input
              value={khachHang.hoTen || ""}
              onChange={(e) => setKhachHang({ ...khachHang, hoTen: e.target.value })}
              placeholder="Nhập họ tên"
            />

            <label>Email:</label>
            <input
              value={khachHang.email || ""}
              onChange={(e) => setKhachHang({ ...khachHang, email: e.target.value })}
              placeholder="Nhập email"
            />

            <label>Số điện thoại:</label>
            <input
              value={khachHang.soDienThoai || ""}
              onChange={(e) => setKhachHang({ ...khachHang, soDienThoai: e.target.value })}
              placeholder="Nhập số điện thoại"
            />

            <label>Địa chỉ:</label>
            <input
              value={khachHang.diaChiText || ""}
              onChange={(e) => setKhachHang({ ...khachHang, diaChiText: e.target.value })}
              placeholder="Nhập địa chỉ"
            />

            <label>Ghi chú:</label>
            <textarea
              value={khachHang.ghiChu || ""}
              onChange={(e) => setKhachHang({ ...khachHang, ghiChu: e.target.value })}
              placeholder="Thêm ghi chú..."
            />
          </div>

          <button onClick={handleUpdate} className="save-btn">
            💾 Cập nhật
          </button>
        </div>
      )}

      {/* ================== LỊCH SỬ ================== */}
      {tab === "history" && (
        <div className="section-box">
          <h2>📜 Lịch sử giao dịch</h2>

          <h3>🟢 Bán phế liệu</h3>
          <table>
            <thead>
              <tr>
                <th>Tên phế liệu</th>
                <th>Khối lượng</th>
                <th>Đơn giá</th>
                <th>Tổng tiền</th>
              </tr>
            </thead>
            <tbody>
              {lichSuBan.map((b, i) => (
                <tr key={i}>
                  <td>{b.tenPheLieu}</td>
                  <td>{b.khoiLuong} kg</td>
                  <td>{b.donGia?.toLocaleString()} đ</td>
                  <td>{(b.khoiLuong * b.donGia).toLocaleString()} đ</td>
                </tr>
              ))}
            </tbody>
          </table>

          <h3>🔵 Mua phế liệu</h3>
          <table>
            <thead>
              <tr>
                <th>Tên phế liệu</th>
                <th>Khối lượng</th>
                <th>Giá đề xuất</th>
              </tr>
            </thead>
            <tbody>
              {lichSuMua.map((m, i) => (
                <tr key={i}>
                  <td>{m.tenPheLieu}</td>
                  <td>{m.khoiLuong} kg</td>
                  <td>{m.donGiaDeXuat?.toLocaleString()} đ</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {/* ================== DOANH NGHIỆP ================== */}
      {tab === "search" && (
        <div className="section-box">
          <h2>🏢 Tìm doanh nghiệp thu mua</h2>
          <input
            type="text"
            placeholder="Nhập tên doanh nghiệp..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
          <div className="doanhnghiep-list">
            {filteredDoanhNghiep.map((d) => (
              <div key={d.id} className="doanhnghiep-item">
                <h4>{d.tenDoanhNghiep}</h4>
                <p>📍 {d.diaChiText}</p>
                <p>📞 {d.soDienThoai}</p>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* ================== BÁN PHẾ LIỆU ================== */}
      {tab === "ban" && (
        <div className="section-box">
          <h2>♻️ Đăng bài bán phế liệu</h2>
          <div className="form-grid">
            <label>Loại phế liệu:</label>
            <select value={selectedLoai} onChange={(e) => handleLoaiChange(e.target.value)}>
              <option value="">-- Chọn loại --</option>
              {loaiList.map((l) => (
                <option key={l.maLoai} value={l.maLoai}>
                  {l.tenLoai}
                </option>
              ))}
            </select>

            <label>Phế liệu:</label>
            <select
              value={selectedPheLieu}
              onChange={(e) => handlePheLieuChange(e.target.value)}
              disabled={!selectedLoai}
            >
              <option value="">-- Chọn phế liệu --</option>
              {pheLieuList
                .filter((p) => p.maLoai === selectedLoai)
                .map((p) => (
                  <option key={p.id} value={p.id}>
                    {p.tenPheLieu}
                  </option>
                ))}
            </select>

            <label>Khối lượng (kg):</label>
            <input
              type="number"
              value={khoiLuong}
              onChange={(e) => setKhoiLuong(e.target.value)}
            />

            <label>Mô tả:</label>
            <textarea
              value={moTa}
              onChange={(e) => setMoTa(e.target.value)}
              placeholder="Thêm ghi chú..."
            />
          </div>

          {selectedItem && (
            <div className="preview-box">
              <p>
                <b>Đơn giá:</b> {selectedItem.donGia?.toLocaleString()} đ/kg
              </p>
              <p>
                <b>Mô tả:</b> {selectedItem.moTa}
              </p>
            </div>
          )}
          <button onClick={handleDangBan} className="save-btn">
            🚀 Đăng bài bán
          </button>
        </div>
      )}

      {/* ================== MUA PHẾ LIỆU ================== */}
      {tab === "mua" && (
        <div className="section-box">
          <h2>💸 Đăng bài mua phế liệu</h2>
          <div className="form-grid">
            <label>Loại phế liệu:</label>
            <select value={selectedLoai} onChange={(e) => handleLoaiChange(e.target.value)}>
              <option value="">-- Chọn loại --</option>
              {loaiList.map((l) => (
                <option key={l.maLoai} value={l.maLoai}>
                  {l.tenLoai}
                </option>
              ))}
            </select>

            <label>Phế liệu:</label>
            <select
              value={selectedPheLieu}
              onChange={(e) => handlePheLieuChange(e.target.value)}
              disabled={!selectedLoai}
            >
              <option value="">-- Chọn phế liệu --</option>
              {pheLieuList
                .filter((p) => p.maLoai === selectedLoai)
                .map((p) => (
                  <option key={p.id} value={p.id}>
                    {p.tenPheLieu}
                  </option>
                ))}
            </select>

            <label>Khối lượng (kg):</label>
            <input
              type="number"
              value={khoiLuong}
              onChange={(e) => setKhoiLuong(e.target.value)}
            />

            <label>Giá đề xuất (đ/kg):</label>
            <input
              type="number"
              value={giaDeXuat}
              onChange={(e) => setGiaDeXuat(e.target.value)}
            />

            <label>Mô tả:</label>
            <textarea value={moTa} onChange={(e) => setMoTa(e.target.value)} />
          </div>

          <button onClick={handleDangMua} className="save-btn">
            💰 Đăng bài mua
          </button>
        </div>
      )}
    </div>
  );
}
