import { useEffect, useState } from "react";
import axiosClient from "../../api/axiosClient";

export default function LoaiPheLieuDashboard() {
  const [data, setData] = useState([]);

  useEffect(() => {
    axiosClient.get("/loaiphelieu").then((res) => setData(res.data));
  }, []);

  return (
    <div className="p-8">
      <h1 className="text-2xl font-bold text-center mb-6 text-green-700">
        🧩 Danh sách Loại Phế Liệu
      </h1>
      <table className="min-w-full bg-white shadow-md rounded-lg overflow-hidden">
        <thead className="bg-green-500 text-white">
          <tr>
            <th className="p-3 text-left">Mã loại</th>
            <th className="p-3 text-left">Tên loại</th>
            <th className="p-3 text-left">Mô tả</th>
          </tr>
        </thead>
        <tbody>
          {data.map((item) => (
            <tr key={item.maLoai} className="border-b hover:bg-gray-100">
              <td className="p-3">{item.maLoai}</td>
              <td className="p-3">{item.tenLoai}</td>
              <td className="p-3">{item.moTa}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
