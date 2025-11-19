// ✅ Подключаем ВСЕ необходимые нам "запчасти"
using ElectronicsStore.DAL;
using ElectronicsStore.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Подключаем Базу Данных (PostgreSQL) ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(connectionString));

// --- 2. Подключаем "движок" Identity ---
// Он будет управлять пользователями, паролями и ролями
// Мы говорим ему: "Используй наш класс ApplicationUser и нашу базу ElectronicsStoreContext"
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ElectronicsStoreContext>()
    .AddDefaultTokenProviders();

// --- 3. Настраиваем "Куки" (Cookies) ---
// Это нужно, чтобы SignInManager.IsSignedIn(User) в _Layout.cshtml заработал
builder.Services.ConfigureApplicationCookie(options =>
{
    // Если пользователь не вошел и пытается зайти туда, куда нельзя,
    // его перекинет на главную страницу (где откроется модальное окно)
    options.LoginPath = "/";
    options.AccessDeniedPath = "/";
    options.SlidingExpiration = true;
});

// --- 4. Стандартные сервисы MVC ---
builder.Services.AddControllersWithViews();
// (Тут мы в будущем добавим сервисы из BLL, как в отчете Жени)


var app = builder.Build();

// --- 5. Конвейер (Pipeline) ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ✅ ВАЖНЫЙ ПОРЯДОК: Сначала "Аутентификация", потом "Авторизация"
app.UseAuthentication(); // Проверяем, кто ты (читаем "куки")
app.UseAuthorization();  // Проверяем, что тебе можно делать

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();