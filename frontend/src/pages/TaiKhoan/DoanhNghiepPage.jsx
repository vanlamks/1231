import { useEffect, useState } from "react";
import axiosClient from "../../api/axiosClient";

export default function DoanhNghiepPage() {
  const [list, setList] = useState([]);

  useEffect(() => {
    fetchData();
  }, []);

  const fetchData = async () => {
    try {
      const res = await axiosClient.get("/DoanhNghiep");
      setList(res.data);
    } catch (err) {
      console.error("❌ Lỗi tải danh sách doanh nghiệp:", err);
    }
  };

  return (
    <div className="p-6 bg-gray-50 min-h-screen">
      <h1 className="text-3xl font-bold text-indigo-700 mb-6 text-center">
        🏢 Trang Doanh nghiệp
      </h1>

      <div className="bg-white shadow-lg rounded-lg p-4">
        <table className="w-full border border-gray-200">
          <thead className="bg-gray-100">
            <tr>
              <th>Tên doanh nghiệp</th>
              <th>Email</th>
              <th>Mã số thuế</th>
              <th>Website</th>
              <th>Trạng thái</th>
            </tr>
          </thead>
          <tbody>
            {list.map((dn) => (
              <tr key={dn.id} className="text-center border-t">
                <td>{dn.tenDoanhNghiep}</td>
                <td>{dn.email}</td>
                <td>{dn.maSoThue}</td>
                <td>
                  {dn.website ? (
                    <a
                      href={dn.website}
                      target="_blank"
                      rel="noopener noreferrer"
                      className="text-blue-500 underline"
                    >
                      {dn.website}
                    </a>
                  ) : (
                    "—"
                  )}
                </td>
                <td>{dn.verified ? "✅ Đã xác minh" : "🕓 Chờ duyệt"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
