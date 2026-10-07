using BookManagerApp.Data;
using BookManagerApp.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Đăng ký DbContext kết nối PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ------------------------------------------------------------------------
// BƯỚC 4: ĐĂNG KÝ REQUEST LOGGING MIDDLEWARE VÀO PIPELINE
// (Đặt trước UseRouting để chặn URL không hợp lệ trước khi vào hệ thống Route)
// ------------------------------------------------------------------------
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting();

app.UseAuthorization();

// Thiết lập Route mặc định mở trang Danh sách sách (Books/Index) khi chạy app
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

app.Run();