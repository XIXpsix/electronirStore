using ElectronicsStore;
using ElectronicsStore.DAL;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Domain.Entity; // Для User, если нужно
using Microsoft.AspNetCore.Authentication.Cookies; // Для Cookie
using Microsoft.AspNetCore.Authentication.Google; // Для Google

var builder = WebApplication.CreateBuilder(args);

// 1. Подключение к БД PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Инициализация репозиториев и сервисов (ваш класс Initializer)
builder.Services.InitializeRepositories();
builder.Services.InitializeServices();

// 3. Добавление контроллеров с представлениями
builder.Services.AddControllersWithViews();

// 4. Настройка Аутентификации (Cookies + Google)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

var app = builder.Build();

// 5. Middleware (Порядок важен!)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // <-- Сначала проверяем "кто это"
app.UseAuthorization();  // <-- Потом "что можно"

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();