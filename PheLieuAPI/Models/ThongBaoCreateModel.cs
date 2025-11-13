namespace PheLieuAPI.Models
{
    public class ThongBaoCreateModel
    {
        public Guid? DoanhNghiepId { get; set; }
        public string Loai { get; set; } = string.Empty;
        public Guid? DonBanId { get; set; }
        public Guid? DonMuaId { get; set; }
        public string NoiDung { get; set; } = string.Empty;
    }
}
