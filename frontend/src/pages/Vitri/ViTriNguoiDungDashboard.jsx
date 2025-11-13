import { useEffect, useState } from "react";
import axiosClient from "../../api/axiosClient";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import L from "leaflet";

// Tùy chỉnh icon để không lỗi path
delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl:
    "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon-2x.png",
  iconUrl:
    "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon.png",
  shadowUrl:
    "https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-shadow.png",
});

export default function ViTriNguoiDungDashboard() {
  const [list, setList] = useState([]);
  const [message, setMessage] = useState("");

  const loadData = async () => {
    try {
      const res = await axiosClient.get("/vitringuoidung");
      setList(res.data);
    } catch (err) {
      console.error(err);
      setMessage("❌ Lỗi tải dữ liệu vị trí người dùng!");
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  return (
    <div className="p-8 bg-gradient-to-b from-green-50 to-blue-50 min-h-screen">
      <h1 className="text-3xl font-bold text-center text-green-700 mb-6">
        📍 Vị trí người dùng (Leaflet Map)
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

      <MapContainer
        center={[10.776, 106.7]} // Tọa độ trung tâm HCM
        zoom={13}
        scrollWheelZoom={true}
      >
        <TileLayer
          attribution="&copy; OpenStreetMap contributors"
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />

        {list.map((item) => (
          <Marker key={item.id} position={[item.viDo, item.kinhDo]}>
            <Popup>
              <b>Email:</b> {item.email} <br />
              <b>Kinh độ:</b> {item.kinhDo} <br />
              <b>Vĩ độ:</b> {item.viDo} <br />
              <b>Cập nhật:</b>{" "}
              {new Date(item.thoiGianCapNhat).toLocaleString("vi-VN")}
            </Popup>
          </Marker>
        ))}
      </MapContainer>
    </div>
  );
}
