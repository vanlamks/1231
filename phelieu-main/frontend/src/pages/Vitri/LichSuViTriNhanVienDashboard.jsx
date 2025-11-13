import { useEffect, useState } from "react";
import axiosClient from "../../api/axiosClient";
import { MapContainer, TileLayer, Marker, Popup, Polyline } from "react-leaflet";
import L from "leaflet";

delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl:
    "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon-2x.png",
  iconUrl:
    "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon.png",
  shadowUrl:
    "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-shadow.png",
});

export default function LichSuViTriNhanVienDashboard() {
  const [list, setList] = useState([]);
  const [message, setMessage] = useState("");

  const loadData = async () => {
    try {
      const res = await axiosClient.get("/lichsuvitrinhanvien");
      setList(res.data);
    } catch (err) {
      console.error(err);
      setMessage("❌ Lỗi tải dữ liệu lịch sử vị trí nhân viên!");
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  // Gom nhóm các điểm theo nhân viên (để vẽ polyline)
  const groupedByNhanVien = list.reduce((acc, curr) => {
    if (!acc[curr.tenNhanVien]) acc[curr.tenNhanVien] = [];
    acc[curr.tenNhanVien].push([curr.viDo, curr.kinhDo]);
    return acc;
  }, {});

  return (
    <div className="p-8 bg-gradient-to-b from-blue-50 to-green-50 min-h-screen">
      <h1 className="text-3xl font-bold text-center text-blue-700 mb-6">
        👷‍♂️ Lịch sử di chuyển nhân viên
      </h1>

      {message && (
        <p
          className={`text-center mb-4 ${
            message.includes("✅") ? "text-green-600" : "text-red-500"
          }`}
        >
          {message}
        </p>
      )}

      <MapContainer center={[10.776, 106.7]} zoom={13} scrollWheelZoom={true}>
        <TileLayer
          attribution="&copy; OpenStreetMap contributors"
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />

        {Object.keys(groupedByNhanVien).map((nv, idx) => (
          <Polyline
            key={nv}
            positions={groupedByNhanVien[nv]}
            pathOptions={{
              color: ["red", "blue", "green", "purple", "orange"][idx % 5],
              weight: 4,
              opacity: 0.7,
            }}
          >
            <Popup>{nv}</Popup>
          </Polyline>
        ))}

        {list.map((item) => (
          <Marker key={item.id} position={[item.viDo, item.kinhDo]}>
            <Popup>
              <b>{item.tenNhanVien}</b> <br />
              🕒 {new Date(item.thoiGian).toLocaleString("vi-VN")} <br />
              📍 ({item.kinhDo}, {item.viDo})
            </Popup>
          </Marker>
        ))}
      </MapContainer>
    </div>
  );
}
