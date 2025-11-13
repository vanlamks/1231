import { useEffect, useState } from "react";
import axiosClient from "../../api/axiosClient";

export default function PheLieuDashboard() {
  const [data, setData] = useState([]);
  const [loaiList, setLoaiList] = useState([]);
  const [form, setForm] = useState({
    tenPheLieu: "",
    maLoai: "",
    khoiLuong: "",
    donGia: "",
    moTa: "",
    hinhAnh: ""
  });
  const [message, setMessage] = useState("");

  // 🧠 Load dữ liệu khi khởi tạo
  useEffect(() => {
    loadData();
    loadLoai();
  }, []);

  const loadData = async () => {
    const res = await axiosClient.get("/phelieu");
    setData(res.data);
  };

  const loadLoai = async () => {
    const res = await axiosClient.get("/loaiphelieu");
    setLoaiList(res.data);
  };

  // 🔧 Xử lý nhập liệu
  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  // ✅ Thêm phế liệu
  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await axiosClient.post("/phelieu", form);
      setMessage("✅ Thêm phế liệu thành công!");
      setForm({
        tenPheLieu: "",
        maLoai: "",
        khoiLuong: "",
        donGia: "",
        moTa: "",
        hinhAnh: ""
      });
      loadData();
    } catch (err) {
      setMessage("❌ Lỗi khi thêm phế liệu!");
    }
  };

  return (
    <div className="p-10 bg-gray-50 min-h-screen">
      <h1 className="text-3xl font-bold text-center mb-8 text-green-700">
        ♻️ Quản lý Phế Liệu
      </h1>

      {/* FORM THÊM */}
      <form
        onSubmit={handleSubmit}
        className="bg-white shadow-md p-6 rounded-lg mb-8 max-w-3xl mx-auto"
      >
        <h2 className="text-xl font-semibold mb-4 text-gray-700">
          ➕ Thêm phế liệu mới
        </h2>

        <div className="grid grid-cols-2 gap-4">
          <input
            type="text"
            name="tenPheLieu"
            value={form.tenPheLieu}
            onChange={handleChange}
            placeholder="Tên phế liệu"
            required
            className="border p-2 rounded"
          />

          <select
            name="maLoai"
            value={form.maLoai}
            onChange={handleChange}
            required
            className="border p-2 rounded"
          >
            <option value="">-- Chọn loại phế liệu --</option>
            {loaiList.map((loai) => (
              <option key={loai.maLoai} value={loai.maLoai}>
                {loai.tenLoai}
              </option>
            ))}
          </select>

          <input
            type="number"
            step="0.1"
            name="khoiLuong"
            value={form.khoiLuong}
            onChange={handleChange}
            placeholder="Khối lượng (kg)"
            required
            className="border p-2 rounded"
          />

          <input
            type="number"
            step="100"
            name="donGia"
            value={form.donGia}
            onChange={handleChange}
            placeholder="Đơn giá (VNĐ)"
            required
            className="border p-2 rounded"
          />

          <input
            type="text"
            name="moTa"
            value={form.moTa}
            onChange={handleChange}
            placeholder="Mô tả"
            className="border p-2 rounded col-span-2"
          />

          <input
            type="text"
            name="hinhAnh"
            value={form.hinhAnh}
            onChange={handleChange}
            placeholder="Tên file hình (vd: satthep1.jpg)"
            className="border p-2 rounded col-span-2"
          />
        </div>

        <button
          type="submit"
          className="mt-5 w-full bg-green-600 text-white py-2 rounded hover:bg-green-700 transition"
        >
          Thêm mới
        </button>

        {message && (
          <p className="mt-3 text-center text-sm font-medium text-green-600">
            {message}
          </p>
        )}
      </form>

      {/* DANH SÁCH */}
      <table className="min-w-full bg-white shadow-lg rounded-lg overflow-hidden">
        <thead className="bg-green-600 text-white">
          <tr>
            <th className="p-3 text-left">Tên phế liệu</th>
            <th className="p-3 text-left">Loại</th>
            <th className="p-3 text-left">Khối lượng (kg)</th>
            <th className="p-3 text-left">Đơn giá (VNĐ)</th>
            <th className="p-3 text-left">Mô tả</th>
            <th className="p-3 text-left">Hình ảnh</th>
          </tr>
        </thead>
        <tbody>
          {data.map((item) => (
            <tr key={item.id} className="border-b hover:bg-gray-100">
              <td className="p-3">{item.tenPheLieu}</td>
              <td className="p-3">{item.tenLoai}</td>
              <td className="p-3">{item.khoiLuong}</td>
              <td className="p-3">{item.donGia.toLocaleString()} đ</td>
              <td className="p-3">{item.moTa}</td>
              <td className="p-3">
                <img
                  src={`/images/${item.hinhAnh}`}
                  alt={item.tenPheLieu}
                  className="w-16 h-16 rounded-md object-cover border"
                />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
