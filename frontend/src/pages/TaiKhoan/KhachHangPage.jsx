 import { useEffect, useState } from "react";
 import axiosClient from "../api/axiosClient";
 
 export default function KhachHangPage() {
   const [list, setList] = useState([]);
 
   useEffect(() => {
     fetchData();
   }, []);
 
   const fetchData = async () => {
     try {
       const res = await axiosClient.get("/KhachHang");
       setList(res.data);
     } catch (err) {
       console.error("❌ Lỗi tải danh sách khách hàng:", err);
     }
   };
 
   return (
     <div className="p-6 bg-gray-50 min-h-screen">
       <h1 className="text-3xl font-bold text-orange-600 mb-6 text-center">
         👩‍🦰 Trang Khách hàng
       </h1>
 
       <div className="bg-white shadow-lg rounded-lg p-4">
         <table className="w-full border border-gray-200">
           <thead className="bg-gray-100">
             <tr>
               <th>Họ tên</th>
               <th>Email</th>
               <th>Địa chỉ</th>
               <th>Ghi chú</th>
             </tr>
           </thead>
           <tbody>
             {list.map((kh) => (
               <tr key={kh.id} className="text-center border-t">
                 <td>{kh.hoTen}</td>
                 <td>{kh.email}</td>
                 <td>{kh.diaChiText}</td>
                 <td>{kh.ghiChu || "—"}</td>
               </tr>
             ))}
           </tbody>
         </table>
       </div>
     </div>
   );
 }
 