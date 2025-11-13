import { useEffect, useState } from "react";
import axiosClient from "../../api/axiosClient";

export default function AdminPage() {
  const [accounts, setAccounts] = useState([]);

  useEffect(() => {
    fetchAccounts();
  }, []);

  const fetchAccounts = async () => {
    try {
      const res = await axiosClient.get("/TaiKhoan");
      setAccounts(res.data);
    } catch (err) {
      console.error("❌ Lỗi tải danh sách tài khoản:", err);
    }
  };

  return (
    <div className="p-6 bg-gray-50 min-h-screen">
      <h1 className="text-3xl font-bold text-center text-blue-700 mb-6">
        🛠️ Trang Quản trị (Admin)
      </h1>

      <div className="bg-white shadow-lg rounded-lg p-4">
        <table className="w-full border border-gray-200">
          <thead className="bg-gray-100">
            <tr>
              <th>Email</th>
              <th>SĐT</th>
              <th>Vai trò</th>
              <th>Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            {accounts.map((acc) => (
              <tr key={acc.id} className="text-center border-t">
                <td>{acc.email}</td>
                <td>{acc.soDienThoai}</td>
                <td>{acc.vaiTro}</td>
                <td>{acc.trangThaiHoatDong ? "✅ Hoạt động" : "❌ Khóa"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
