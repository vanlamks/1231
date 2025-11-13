import { useEffect, useState } from "react";
import axiosClient from "../../api/axiosClient";

export default function DonBanPheLieuDashboard() {
  const [list, setList] = useState([]);
  const [message, setMessage] = useState("");
  const [doanhNghieps, setDoanhNghieps] = useState([]);
  const [khachHangs, setKhachHangs] = useState([]);
  const [nguoiDang, setNguoiDang] = useState("DOANH_NGHIEP"); // Mặc định là DN
  const [form, setForm] = useState({
    khachHangId: "",
    doanhNghiepId: "",
    tenPheLieu: "",
    khoiLuong: "",
    donGia: "",
    moTa: "",
  });

  // 🔹 Load dữ liệu ban đầu
  const loadData = async () => {
    try {
      const [resDon, resDN, resKH] = await Promise.all([
        axiosClient.get("/DonBanPheLieu"),
        axiosClient.get("/DoanhNghiep"),
        axiosClient.get("/KhachHang"),
      ]);
      setList(resDon.data);
      setDoanhNghieps(resDN.data);
      setKhachHangs(resKH.data);
    } catch (err) {
      console.error(err);
      setMessage("❌ Lỗi tải dữ liệu!");
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  // 🔹 Thêm đơn bán (tùy loại người đăng)
  const handleAdd = async (e) => {
    e.preventDefault();
    try {
      const body =
        nguoiDang === "DOANH_NGHIEP"
          ? {
              DoanhNghiepId: form.doanhNghiepId,
              TenPheLieu: form.tenPheLieu,
              KhoiLuong: parseFloat(form.khoiLuong),
              DonGia: parseFloat(form.donGia),
              MoTa: form.moTa,
            }
          : {
              KhachHangId: form.khachHangId,
              TenPheLieu: form.tenPheLieu,
              KhoiLuong: parseFloat(form.khoiLuong),
              DonGia: parseFloat(form.donGia),
              MoTa: form.moTa,
            };

      await axiosClient.post("/DonBanPheLieu", body);
      setMessage("✅ Thêm đơn bán thành công!");
      setForm({
        khachHangId: "",
        doanhNghiepId: "",
        tenPheLieu: "",
        khoiLuong: "",
        donGia: "",
        moTa: "",
      });
      loadData();
    } catch (err) {
      console.error(err);
      setMessage("❌ Lỗi thêm đơn bán!");
    }
  };

  // 🔹 Cập nhật trạng thái
  const handleUpdate = async (id, trangThai) => {
    try {
      await axiosClient.put(`/DonBanPheLieu/${id}`, {
        khoiLuong: 0,
        donGia: 0,
        moTa: "",
        trangThai,
      });
      setMessage("✅ Cập nhật trạng thái thành công!");
      loadData();
    } catch (err) {
      console.error(err);
      setMessage("❌ Lỗi cập nhật!");
    }
  };

  // 🔹 Xóa đơn
  const handleDelete = async (id) => {
    if (!window.confirm("Bạn chắc chắn muốn xóa đơn này?")) return;
    try {
      await axiosClient.delete(`/DonBanPheLieu/${id}`);
      setMessage("🗑️ Xóa thành công!");
      loadData();
    } catch (err) {
      console.error(err);
      setMessage("❌ Lỗi khi xóa!");
    }
  };

  return (
    <div className="p-8 bg-gradient-to-b from-yellow-50 to-yellow-100 min-h-screen">
      <h1 className="text-3xl font-bold text-center text-yellow-700 mb-6">
        💰 Quản lý Đơn Bán Phế Liệu
      </h1>

      {message && (
        <div
          className={`text-center mb-4 font-semibold ${
            message.includes("✅") ? "text-green-600" : "text-red-500"
          }`}
        >
          {message}
        </div>
      )}

      {/* 🔹 Bộ chọn loại người đăng */}
      <div className="flex justify-center mb-6 space-x-6">
        <label className="flex items-center space-x-2">
          <input
            type="radio"
            value="DOANH_NGHIEP"
            checked={nguoiDang === "DOANH_NGHIEP"}
            onChange={() => setNguoiDang("DOANH_NGHIEP")}
          />
          <span>Doanh nghiệp</span>
        </label>
        <label className="flex items-center space-x-2">
          <input
            type="radio"
            value="KHACH_HANG"
            checked={nguoiDang === "KHACH_HANG"}
            onChange={() => setNguoiDang("KHACH_HANG")}
          />
          <span>Khách hàng</span>
        </label>
      </div>

      {/* 🔹 Form thêm */}
      <form
        onSubmit={handleAdd}
        className="bg-white p-5 rounded-xl shadow-lg mb-8 grid grid-cols-2 gap-4"
      >
        {nguoiDang === "DOANH_NGHIEP" ? (
          <select
            name="doanhNghiepId"
            value={form.doanhNghiepId}
            onChange={handleChange}
            required
            className="border rounded p-2"
          >
            <option value="">-- Chọn doanh nghiệp --</option>
            {doanhNghieps.map((d) => (
              <option key={d.Id} value={d.Id}>
                {d.TenDoanhNghiep}
              </option>
            ))}
          </select>
        ) : (
          <select
            name="khachHangId"
            value={form.khachHangId}
            onChange={handleChange}
            required
            className="border rounded p-2"
          >
            <option value="">-- Chọn khách hàng --</option>
            {khachHangs.map((k) => (
              <option key={k.Id} value={k.Id}>
                {k.HoTen}
              </option>
            ))}
          </select>
        )}

        <input
          type="text"
          name="tenPheLieu"
          placeholder="Tên phế liệu"
          value={form.tenPheLieu}
          onChange={handleChange}
          required
          className="border rounded p-2"
        />
        <input
          type="number"
          name="khoiLuong"
          placeholder="Khối lượng (kg)"
          value={form.khoiLuong}
          onChange={handleChange}
          required
          className="border rounded p-2"
        />
        <input
          type="number"
          name="donGia"
          placeholder="Đơn giá (VNĐ/kg)"
          value={form.donGia}
          onChange={handleChange}
          required
          className="border rounded p-2"
        />
        <input
          type="text"
          name="moTa"
          placeholder="Mô tả"
          value={form.moTa}
          onChange={handleChange}
          className="col-span-2 border rounded p-2"
        />
        <button
          type="submit"
          className="col-span-2 bg-yellow-600 hover:bg-yellow-700 text-white py-2 rounded-lg"
        >
          ➕ Thêm Đơn Bán
        </button>
      </form>

      {/* 🔹 Bảng danh sách */}
      <div className="overflow-x-auto bg-white rounded-xl shadow-lg">
        <table className="min-w-full border border-gray-200">
          <thead className="bg-yellow-600 text-white">
            <tr>
              <th className="p-3 text-left">Người đăng</th>
              <th className="p-3 text-left">Loại</th>
              <th className="p-3 text-left">Tên phế liệu</th>
              <th className="p-3 text-left">Khối lượng</th>
              <th className="p-3 text-left">Đơn giá</th>
              <th className="p-3 text-left">Mô tả</th>
              <th className="p-3 text-left">Trạng thái</th>
              <th className="p-3 text-center">Thao tác</th>
            </tr>
          </thead>
          <tbody>
            {list.length === 0 ? (
              <tr>
                <td colSpan="8" className="text-center p-4 text-gray-500">
                  Không có dữ liệu.
                </td>
              </tr>
            ) : (
              list.map((d) => (
                <tr key={d.Id} className="border-b hover:bg-yellow-50">
                  <td className="p-3">{d.NguoiDang || d.TenDoanhNghiep}</td>
                  <td className="p-3">{d.LoaiNguoiDang || "—"}</td>
                  <td className="p-3">{d.TenPheLieu}</td>
                  <td className="p-3">{d.KhoiLuong}</td>
                  <td className="p-3">
                    {d.DonGia ? d.DonGia.toLocaleString() + " đ" : "—"}
                  </td>
                  <td className="p-3">{d.MoTa}</td>
                  <td className="p-3">
                    <select
                      defaultValue={d.TrangThai}
                      onChange={(e) => handleUpdate(d.Id, e.target.value)}
                      className="border rounded p-1"
                    >
                      <option>Chờ giao dịch</option>
                      <option>Đang thương lượng</option>
                      <option>Đã hoàn tất</option>
                    </select>
                  </td>
                  <td className="p-3 text-center">
                    <button
                      onClick={() => handleDelete(d.Id)}
                      className="bg-red-500 hover:bg-red-600 text-white px-3 py-1 rounded"
                    >
                      Xóa
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
