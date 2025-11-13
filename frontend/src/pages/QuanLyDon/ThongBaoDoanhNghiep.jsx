import { useEffect, useState } from "react";
import axiosClient from "../../api/axiosClient";

export default function ThongBaoDoanhNghiep() {
  const doanhNghiepId = localStorage.getItem("doanhnghiep_id");
  const [list, setList] = useState([]);

  const loadData = async () => {
    try {
      const res = await axiosClient.get(
        `/ThongBao/DoanhNghiep/${doanhNghiepId}`
      );
      setList(res.data);
    } catch (err) {
      console.error("Lỗi tải thông báo:", err);
    }
  };

  const markAsRead = async (id) => {
    await axiosClient.put(`/ThongBao/Read/${id}`);
    loadData();
  };

  useEffect(() => {
    loadData();
    const timer = setInterval(loadData, 5000);
    return () => clearInterval(timer);
  }, []);

  return (
    <div className="p-8 bg-white min-h-screen">
      <h1 className="text-3xl font-bold text-blue-600 mb-6">
        🔔 Thông báo giao dịch
      </h1>

      {list.length === 0 ? (
        <p className="text-center text-gray-500">Không có thông báo.</p>
      ) : (
        list.map((tb) => (
          <div
            key={tb.Id}
            className={`p-4 rounded-lg shadow mb-3 ${
              tb.DaXem ? "bg-gray-200" : "bg-blue-50"
            }`}
          >
            <p className="text-lg">{tb.NoiDung}</p>
            <p className="text-sm text-gray-600">
              {new Date(tb.CreatedAt).toLocaleString()}
            </p>

            {!tb.DaXem && (
              <button
                className="mt-2 bg-blue-600 text-white px-3 py-1 rounded"
                onClick={() => markAsRead(tb.Id)}
              >
                Đánh dấu đã xem
              </button>
            )}
          </div>
        ))
      )}
    </div>
  );
}
