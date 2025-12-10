using ElectronicsStore;
using ElectronicsStore.DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Подключение БД
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(connectionString));

// Добавление MVC
builder.Services.AddControllersWithViews();

// Аутентификация (Cookie)
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", config =>
    {
        config.LoginPath = "/Account/Login";
        config.AccessDeniedPath = "/Account/AccessDenied";
    });

// === ВОТ ЗДЕСЬ ИСПРАВЛЕНИЕ ===
// Используем builder.Services, а не просто services
Initializer.InitializeRepositories(builder.Services);
Initializer.InitializeServices(builder.Services);
// ============================

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Обязательно перед Authorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();