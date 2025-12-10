using ElectronicsStore;
using ElectronicsStore.DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ИСПРАВЛЕНИЕ: Гарантируем, что строка подключения не null
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Строка подключения 'DefaultConnection' не найдена в appsettings.json");
}

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

// Подключение сервисов
Initializer.InitializeRepositories(builder.Services);
Initializer.InitializeServices(builder.Services);

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();