using ElectronicsStore;
using ElectronicsStore.DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Получаем строку подключения с проверкой
// Если её нет, программа сразу скажет об этом, а не упадет молча
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ОШИБКА: Строка подключения 'DefaultConnection' не найдена в appsettings.json!");

// 2. Подключаем Базу Данных
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Добавляем MVC (Контроллеры и Представления)
builder.Services.AddControllersWithViews();

// 4. Настраиваем Аутентификацию (Вход/Регистрация)
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", config =>
    {
        config.LoginPath = "/Account/Login";
        config.AccessDeniedPath = "/Account/AccessDenied";
    });

// 5. Инициализация наших сервисов (из файла Initializer.cs)
Initializer.InitializeRepositories(builder.Services);
Initializer.InitializeServices(builder.Services);

// --- Сборка приложения (Ошибка падает здесь, если пункты выше не сработали) ---
var app = builder.Build();

// 6. Настройка конвейера (Pipeline)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Кто ты?
app.UseAuthorization();  // Можно ли тебе сюда?

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();