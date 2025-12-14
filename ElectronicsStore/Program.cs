using ElectronicsStore; // Проверьте, что Initializer в этом namespace
using ElectronicsStore.DAL;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.DAL.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

// 1. Подключение к БД
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Инициализация слоев
// Теперь эти методы будут доступны, т.к. Initializer.cs исправлен
builder.Services.InitializeRepositories();
builder.Services.InitializeServices();
builder.Services.InitializeValidators();

// 3. Контроллеры
builder.Services.AddControllersWithViews();

// 4. Аутентификация
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
    // Безопасное получение настроек с проверкой на null (опционально, но полезно)
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
});

var app = builder.Build();

// === ЗАПУСК ИНИЦИАЛИЗАЦИИ ДАННЫХ ===
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Теперь передаем просто services
        await Initializer.InitializeData(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при заполнении базы данных.");
    }
}
// ===================================

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