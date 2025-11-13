import { useEffect, useState } from "react";
import axiosClient from "../../api/axiosClient";

export default function LichHenDashboard() {
  const [lichHenList, setLichHenList] = useState([]);
  const [trangThaiList, setTrangThaiList] = useState([]);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState("");

  // 🧩 Lấy danh sách lịch hẹn
  const loadLichHen = async () => {
    try {
      setLoading(true);
      const res = await axiosClient.get("/lichhen");
      setLichHenList(res.data);
    } catch (err) {
      console.error(err);
      setMessage("❌ Không thể tải danh sách lịch hẹn!");
    } finally {
      setLoading(false);
    }
  };

  // 🧩 Lấy danh sách trạng thái
  const loadTrangThai = async () => {
    try {
      const res = await axiosClient.get("/trangthailichhen");
      setTrangThaiList(res.data);
    } catch (err) {
      console.error(err);
      setMessage("❌ Không thể tải danh sách trạng thái!");
    }
  };

  // 🔄 Cập nhật trạng thái lịch hẹn
  const handleUpdate = async (id, trangThaiCode, ghiChu) => {
    try {
      await axiosClient.put(`/lichhen/${id}`, {
        id,
        trangThaiCode,
        ghiChu,
      });
      setMessage("✅ Cập nhật trạng thái thành công!");
      loadLichHen();
    } catch (err) {
      console.error(err);
      setMessage("❌ Lỗi khi cập nhật trạng thái!");
    }
  };

  useEffect(() => {
    loadLichHen();
    loadTrangThai();
  }, []);

  return (
    <div className="p-8 bg-gradient-to-b from-green-50 to-green-100 min-h-screen">
      <h1 className="text-3xl font-bold text-center text-green-700 mb-6">
        📅 Quản lý Lịch Hẹn Thu Gom
      </h1>

      {message && (
        <div
          className={`text-center mb-4 font-medium ${
            message.includes("✅")
              ? "text-green-600"
              : "text-red-500"
          }`}
        >
          {message}
        </div>
      )}

      {loading ? (
        <p className="text-center text-gray-600">⏳ Đang tải dữ liệu...</p>
      ) : (
        <div className="overflow-x-auto">
          <table className="min-w-full bg-white border border-gray-200 shadow-lg rounded-lg">
            <thead className="bg-green-600 text-white">
              <tr>
                <th className="p-3 text-left">Khách hàng</th>
                <th className="p-3 text-left">Địa chỉ</th>
                <th className="p-3 text-left">Thời gian hẹn</th>
                <th className="p-3 text-left">Trạng thái</th>
                <th className="p-3 text-left">Ghi chú</th>
                <th className="p-3 text-center">Cập nhật</th>
              </tr>
            </thead>
            <tbody>
              {lichHenList.length === 0 ? (
                <tr>
                  <td colSpan="6" className="text-center p-4 text-gray-500">
                    Không có lịch hẹn nào.
                  </td>
                </tr>
              ) : (
                lichHenList.map((item) => (
                  <tr
                    key={item.id}
                    className="border-b hover:bg-green-50 transition"
                  >
                    <td className="p-3 font-medium text-gray-800">
                      {item.tenKhachHang}
                    </td>
                    <td className="p-3">{item.diaChi}</td>
                    <td className="p-3">
                      {new Date(item.thoiGianHen).toLocaleString("vi-VN")}
                    </td>
                    <td className="p-3">
                      <select
                        defaultValue={item.trangThaiCode}
                        onChange={(e) =>
                          handleUpdate(item.id, e.target.value, item.ghiChu)
                        }
                        className="border border-gray-300 rounded p-1"
                      >
                        {trangThaiList.map((tt) => (
                          <option key={tt.code} value={tt.code}>
                            {tt.ten}
                          </option>
                        ))}
                      </select>
                    </td>
                    <td className="p-3 text-gray-600">
                      {item.ghiChu || "—"}
                    </td>
                    <td className="p-3 text-center">
                      <button
                        onClick={() =>
                          handleUpdate(
                            item.id,
                            item.trangThaiCode,
                            item.ghiChu
                          )
                        }
                        className="bg-green-500 hover:bg-green-600 text-white py-1 px-3 rounded shadow"
                      >
                        Lưu
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
