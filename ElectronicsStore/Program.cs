using ElectronicsStore;
using ElectronicsStore.DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- ИСПРАВЛЕНИЕ ОШИБКИ ТУТ ---
// 1. Получаем строку подключения
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Проверяем: если она пустая, выбрасываем понятную ошибку
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Не найдена строка подключения 'DefaultConnection' в файле appsettings.json");
}

// 3. Передаем проверенную строку (компилятор теперь доволен)
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(connectionString));
// -----------------------------

// Добавление MVC
builder.Services.AddControllersWithViews();

// Аутентификация (Cookie)
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", config =>
    {
        config.LoginPath = "/Account/Login";
        config.AccessDeniedPath = "/Account/AccessDenied";
    });

// Подключение сервисов (Инициализация)
Initializer.InitializeRepositories(builder.Services);
Initializer.InitializeServices(builder.Services);

// Сборка приложения
var app = builder.Build(); // Ошибка должна исчезнуть, так как конфигурация выше теперь корректна

// Настройка конвейера запросов (Pipeline)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Значение HSTS по умолчанию — 30 дней.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Сначала аутентификация
app.UseAuthorization();  // Потом авторизация

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();