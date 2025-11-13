import { useEffect, useState } from "react";
import axiosClient from "../../api/axiosClient";

export default function DonThuGomDashboard() {
  const [donList, setDonList] = useState([]);
  const [trangThaiList, setTrangThaiList] = useState([]);
  const [paymentList, setPaymentList] = useState([]);
  const [message, setMessage] = useState("");

  // 🧩 Load toàn bộ dữ liệu
  const loadData = async () => {
    try {
      const [resDon, resTrangThai, resPayment] = await Promise.all([
        axiosClient.get("/donthugom"),
        axiosClient.get("/TrangThaiDon"),
axiosClient.get("/PaymentMethod"),

      ]);
      setDonList(resDon.data);
      setTrangThaiList(resTrangThai.data);
      setPaymentList(resPayment.data);
    } catch (err) {
      console.error(err);
      setMessage("❌ Lỗi khi tải dữ liệu!");
    }
  };

  // 🔄 Cập nhật trạng thái đơn
  const handleUpdate = async (id, trangThaiCode, phuongThucTT, ghiChu) => {
    try {
      await axiosClient.put(`/donthugom/${id}`, {
        id,
        trangThaiCode,
        phuongThucTT,
        ghiChu,
        tongTien: 0, // nếu không sửa tổng tiền
      });
      setMessage("✅ Cập nhật thành công!");
      loadData();
    } catch (err) {
      console.error(err);
      setMessage("❌ Cập nhật thất bại!");
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  return (
    <div className="p-8 bg-gradient-to-b from-green-50 to-green-100 min-h-screen">
      <h1 className="text-3xl font-bold text-center text-green-700 mb-6">
        🧾 Quản lý Đơn Thu Gom
      </h1>

      {message && (
        <div
          className={`text-center mb-4 font-medium ${
            message.includes("✅") ? "text-green-600" : "text-red-500"
          }`}
        >
          {message}
        </div>
      )}

      <div className="overflow-x-auto">
        <table className="min-w-full bg-white border border-gray-200 shadow-lg rounded-lg">
          <thead className="bg-green-600 text-white">
            <tr>
              <th className="p-3 text-left">Doanh nghiệp</th>
              <th className="p-3 text-left">Nhân viên</th>
              <th className="p-3 text-left">Trạng thái</th>
              <th className="p-3 text-left">Phương thức TT</th>
              <th className="p-3 text-left">Tổng tiền</th>
              <th className="p-3 text-left">Ghi chú</th>
              <th className="p-3 text-center">Cập nhật</th>
            </tr>
          </thead>
          <tbody>
            {donList.length === 0 ? (
              <tr>
                <td colSpan="7" className="text-center p-4 text-gray-500">
                  Không có đơn nào.
                </td>
              </tr>
            ) : (
              donList.map((d) => (
                <tr key={d.id} className="border-b hover:bg-green-50 transition">
                  <td className="p-3">{d.tenDoanhNghiep}</td>
                  <td className="p-3">{d.tenNhanVien}</td>

                  {/* Dropdown trạng thái */}
                  <td className="p-3">
                    <select
                      defaultValue={d.trangThaiCode}
                      onChange={(e) =>
                        handleUpdate(
                          d.id,
                          e.target.value,
                          d.phuongThucTT,
                          d.ghiChu
                        )
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

                  {/* Dropdown phương thức thanh toán */}
                  <td className="p-3">
                    <select
                      defaultValue={d.phuongThucTT}
                      onChange={(e) =>
                        handleUpdate(
                          d.id,
                          d.trangThaiCode,
                          e.target.value,
                          d.ghiChu
                        )
                      }
                      className="border border-gray-300 rounded p-1"
                    >
                      {paymentList.map((p) => (
                        <option key={p.code} value={p.code}>
                          {p.ten}
                        </option>
                      ))}
                    </select>
                  </td>

                  <td className="p-3">{d.tongTien.toLocaleString()} đ</td>
                  <td className="p-3">{d.ghiChu}</td>

                  <td className="p-3 text-center">
                    <button
                      onClick={() =>
                        handleUpdate(
                          d.id,
                          d.trangThaiCode,
                          d.phuongThucTT,
                          "Cập nhật từ dashboard"
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
    </div>
  );
}
