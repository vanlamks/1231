using PheLieuAPI.Helpers;
using PheLieuAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🧩 Đăng ký Service và DbHelper
builder.Services.AddScoped<DbHelper>();
builder.Services.AddScoped<KhachHangService>();
builder.Services.AddScoped<TaiKhoanService>();
builder.Services.AddScoped<DoanhNghiepService>();
builder.Services.AddScoped<NhanVienService>();
builder.Services.AddScoped<AdminUserService>();
builder.Services.AddScoped<PheLieuService>();
builder.Services.AddScoped<LoaiPheLieuService>();
builder.Services.AddScoped<TrangThaiLichHenService>();
builder.Services.AddScoped<LichHenService>();
builder.Services.AddScoped<PaymentMethodService>();
builder.Services.AddScoped<TrangThaiDonService>();
builder.Services.AddScoped<DonThuGomService>();
builder.Services.AddScoped<ViTriNguoiDungService>();
builder.Services.AddScoped<LichSuViTriNhanVienService>();
builder.Services.AddScoped<DonBanPheLieuService>();
builder.Services.AddScoped<DonMuaPheLieuService>();
builder.Services.AddScoped<ThongBaoService>();
builder.Services.AddScoped<LichPhanCongService>();



// CORS để frontend (React) gọi API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Swagger cho môi trường Dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Cấu hình pipeline
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
