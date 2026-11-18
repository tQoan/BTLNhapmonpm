using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QuanLyCuaHangTheThao.Models.Scaffold;
using QuanLyCuaHangTheThao.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Entity Framework Core - Database first (QuanLyCuaHangTheThao)
builder.Services.AddDbContext<QuanLyCuaHangTheThaoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Cookie Authentication + role-based authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=TrangChu}/{action=Index}/{id?}");

// Upgrade legacy plaintext passwords to PBKDF2 hashes (idempotent).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuanLyCuaHangTheThaoContext>();
    var accounts = db.TblTaiKhoans
        .Where(a => a.SMatKhau != null && !a.SMatKhau.StartsWith("PBKDF2:"))
        .ToList();
    foreach (var account in accounts)
    {
        account.SMatKhau = PasswordHasher.Hash(account.SMatKhau);
    }
    if (accounts.Count > 0)
        db.SaveChanges();
}

app.Run();
