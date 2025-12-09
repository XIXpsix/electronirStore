using ElectronicsStore.DAL;
using ElectronicsStore.DAL.Interfaces;   // <-- Добавлено
using ElectronicsStore.DAL.Repositories; // <-- Добавлено
using ElectronicsStore.BLL.Interfaces;   // <-- Добавлено
using ElectronicsStore.BLL.Realizations; // <-- Добавлено
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Подключение БД
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(connectionString));

// ==============================================================
// 2. РЕГИСТРАЦИЯ СЕРВИСОВ И РЕПОЗИТОРИЕВ (ЭТОГО НЕ ХВАТАЛО)
// ==============================================================

// Регистрируем Базовый Репозиторий (для базы данных)
builder.Services.AddScoped(typeof(IBaseStorage<>), typeof(BaseStorage<>));

// Регистрируем твои Сервисы (Товары, Категории)
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Если у тебя есть AccountService, раскомментируй строку ниже:
// builder.Services.AddScoped<IAccountService, AccountService>(); 

// ==============================================================

// 3. Настройка Аутентификации (Куки + Google)
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
    // Проверка, чтобы не падало, если ключей нет в конфиге
    if (googleAuthNSection.Exists())
    {
        options.ClientId = googleAuthNSection["ClientId"];
        options.ClientSecret = googleAuthNSection["ClientSecret"];
    }
});

// 4. Сервисы для MVC (Контроллеры и Представления)
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Настройка конвейера (Pipeline)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Кто ты? (Логин)
app.UseAuthorization();  // Можно ли тебе сюда? (Роль)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();