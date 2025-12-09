using ElectronicsStore.DAL;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.DAL.Repositories;
using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.BLL.Realizations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

// 1. СНАЧАЛА СОЗДАЕМ BUILDER (Это самое важное, ошибка была здесь)
var builder = WebApplication.CreateBuilder(args);

// 2. Подключение к Базе Данных
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Регистрация сервисов и репозиториев
// Базовый репозиторий
builder.Services.AddScoped(typeof(IBaseStorage<>), typeof(BaseStorage<>));

// Твои сервисы (Товары, Категории)
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// 4. Настройка Аутентификации (Вход через Google и Куки)
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
})
.AddGoogle(options =>
{
    IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
    if (googleAuthNSection.Exists())
    {
        options.ClientId = googleAuthNSection["ClientId"];
        options.ClientSecret = googleAuthNSection["ClientSecret"];
    }
});

// 5. Добавляем поддержку контроллеров и представлений (MVC)
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

// ==========================================
// СБОРКА ПРИЛОЖЕНИЯ
var app = builder.Build();
// ==========================================

// Настройка конвейера обработки запросов (Pipeline)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Порядок важен: Сначала Аутентификация (Кто ты?), потом Авторизация (Можно ли тебе?)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();