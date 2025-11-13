import { useEffect, useState } from "react";
import axiosClient from "../../api/axiosClient";

export default function DonMuaPheLieuDashboard() {
  const [list, setList] = useState([]);
  const [message, setMessage] = useState("");
  const [doanhNghieps, setDoanhNghieps] = useState([]);
  const [khachHangs, setKhachHangs] = useState([]);
  const [nguoiDang, setNguoiDang] = useState("DOANH_NGHIEP");

  const [form, setForm] = useState({
    khachHangId: "",
    doanhNghiepId: "",
    tenPheLieu: "",
    khoiLuong: "",
    donGiaDeXuat: "",
    moTa: "",
  });

  const loadData = async () => {
    try {
      const [resDon, resDN, resKH] = await Promise.all([
        axiosClient.get("/DonMuaPheLieu"),
        axiosClient.get("/DoanhNghiep"),
        axiosClient.get("/KhachHang"),
      ]);

      setList(resDon.data);
      setDoanhNghieps(resDN.data);
      setKhachHangs(resKH.data);
    } catch {
      setMessage("❌ Lỗi tải dữ liệu!");
    }
  };

  useEffect(() => loadData(), []);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleAdd = async (e) => {
    e.preventDefault();

    try {
      const body =
        nguoiDang === "DOANH_NGHIEP"
          ? {
              doanhNghiepId: form.doanhNghiepId,
              tenPheLieu: form.tenPheLieu,
              khoiLuong: parseFloat(form.khoiLuong),
              donGiaDeXuat: parseFloat(form.donGiaDeXuat),
              moTa: form.moTa,
            }
          : {
              khachHangId: form.khachHangId,
              tenPheLieu: form.tenPheLieu,
              khoiLuong: parseFloat(form.khoiLuong),
              donGiaDeXuat: parseFloat(form.donGiaDeXuat),
              moTa: form.moTa,
            };

      await axiosClient.post("/DonMuaPheLieu", body);

      setMessage("✅ Thêm đơn mua thành công!");
      setForm({
        khachHangId: "",
        doanhNghiepId: "",
        tenPheLieu: "",
        khoiLuong: "",
        donGiaDeXuat: "",
        moTa: "",
      });

      loadData();
    } catch (err) {
      console.error(err);
      setMessage("❌ Lỗi thêm đơn mua!");
    }
  };

  const handleUpdate = async (id, trangThai) => {
    try {
      await axiosClient.put(`/DonMuaPheLieu/${id}`, { trangThai });
      setMessage("✔ Cập nhật trạng thái thành công!");
      loadData();
    } catch {
      setMessage("❌ Lỗi cập nhật!");
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm("Xóa đơn này?")) return;

    try {
      await axiosClient.delete(`/DonMuaPheLieu/${id}`);
      setMessage("🗑 Xóa thành công!");
      loadData();
    } catch {
      setMessage("❌ Lỗi xóa!");
    }
  };

  return (
    <div className="p-8 bg-green-50 min-h-screen">
      <h1 className="text-3xl font-bold text-center text-green-700 mb-6">
        🛒 Quản lý Đơn Mua Phế Liệu
      </h1>

      {message && (
        <p
          className={`text-center font-semibold mb-3 ${
            message.includes("✔") || message.includes("✅")
              ? "text-green-600"
              : "text-red-500"
          }`}
        >
          {message}
        </p>
      )}

      {/* Chọn loại người đăng */}
      <div className="flex justify-center gap-6 mb-5">
        <label>
          <input
            type="radio"
            value="DOANH_NGHIEP"
            checked={nguoiDang === "DOANH_NGHIEP"}
            onChange={() => setNguoiDang("DOANH_NGHIEP")}
          />{" "}
          Doanh nghiệp
        </label>

        <label>
          <input
            type="radio"
            value="KHACH_HANG"
            checked={nguoiDang === "KHACH_HANG"}
            onChange={() => setNguoiDang("KHACH_HANG")}
          />{" "}
          Khách hàng
        </label>
      </div>

      <form
        onSubmit={handleAdd}
        className="bg-white p-5 rounded-xl shadow-lg grid grid-cols-2 gap-4 mb-8"
      >
        {nguoiDang === "DOANH_NGHIEP" ? (
          <select
            name="doanhNghiepId"
            value={form.doanhNghiepId}
            onChange={handleChange}
            required
            className="border p-2 rounded"
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
            className="border p-2 rounded"
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
          className="border p-2 rounded"
        />

        <input
          type="number"
          name="khoiLuong"
          placeholder="Khối lượng (kg)"
          value={form.khoiLuong}
          onChange={handleChange}
          required
          className="border p-2 rounded"
        />

        <input
          type="number"
          name="donGiaDeXuat"
          placeholder="Giá đề xuất (VNĐ/kg)"
          value={form.donGiaDeXuat}
          onChange={handleChange}
          required
          className="border p-2 rounded"
        />

        <input
          type="text"
          name="moTa"
          placeholder="Mô tả"
          value={form.moTa}
          onChange={handleChange}
          className="col-span-2 border p-2 rounded"
        />

        <button className="col-span-2 bg-green-600 text-white py-2 rounded-lg hover:bg-green-700">
          ➕ Tạo đơn mua
        </button>
      </form>

      {/* Danh sách */}
      <div className="bg-white p-4 shadow-lg rounded-xl overflow-x-auto">
        <table className="min-w-full border">
          <thead className="bg-green-600 text-white">
            <tr>
              <th className="p-3">Người đăng</th>
              <th className="p-3">Loại</th>
              <th className="p-3">Tên</th>
              <th className="p-3">Khối lượng</th>
              <th className="p-3">Giá đề xuất</th>
              <th className="p-3">Mô tả</th>
              <th className="p-3">Trạng thái</th>
              <th className="p-3 text-center">Thao tác</th>
            </tr>
          </thead>

          <tbody>
            {list.length === 0 ? (
              <tr>
                <td colSpan="8" className="text-center p-4">
                  Không có dữ liệu.
                </td>
              </tr>
            ) : (
              list.map((d) => (
                <tr key={d.Id} className="border-b hover:bg-green-50">
                  <td className="p-3">{d.NguoiDang}</td>
                  <td className="p-3">{d.LoaiNguoiDang}</td>
                  <td className="p-3">{d.TenPheLieu}</td>
                  <td className="p-3">{d.KhoiLuong}</td>
                  <td className="p-3">
                    {d.DonGiaDeXuat?.toLocaleString()} đ
                  </td>
                  <td className="p-3">{d.MoTa}</td>

                  <td className="p-3">
                    <select
                      value={d.TrangThai}
                      onChange={(e) => handleUpdate(d.Id, e.target.value)}
                      className="border p-1 rounded"
                    >
                      <option>Đang tìm nguồn cung</option>
                      <option>Đang thương lượng</option>
                      <option>Đã hoàn tất</option>
                    </select>
                  </td>

                  <td className="p-3 text-center">
                    <button
                      onClick={() => handleDelete(d.Id)}
                      className="bg-red-500 text-white px-3 py-1 rounded"
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
