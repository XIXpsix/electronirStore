using ElectronicsStore.DAL;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.DAL.Repositories;
using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.BLL.Realizations;
using ElectronicsStore.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. Подключение к Базе Данных
// ==========================================
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================================
// 2. Регистрация Репозиториев (DAL)
// ==========================================
// Репозитории для работы с таблицами БД
builder.Services.AddScoped<IBaseStorage<Category>, BaseStorage<Category>>();
builder.Services.AddScoped<IBaseStorage<Product>, BaseStorage<Product>>();
builder.Services.AddScoped<IBaseStorage<User>, BaseStorage<User>>(); // <--- Добавлено для пользователей

// ==========================================
// 3. Регистрация Сервисов (BLL)
// ==========================================
// Сервисы с бизнес-логикой
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAccountService, AccountService>(); // <--- Добавлено для регистрации/входа

// ==========================================
// 4. Настройка AutoMapper
// ==========================================
// Ищет классы профилей маппинга (AppMappingProfile) во всей сборке
builder.Services.AddAutoMapper(typeof(Program).Assembly);
// Если профиль лежит в другом проекте (например BLL), раскомментируй строку ниже и добавь ссылку:
// builder.Services.AddAutoMapper(typeof(ElectronicsStore.BLL.AppMappingProfile));

// ==========================================
// 5. Настройка Аутентификации (Cookie)
// ==========================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login"); // Куда перенаправлять неавторизованного пользователя
        options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
    });

// ==========================================
// 6. Добавление контроллеров
// ==========================================
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ==========================================
// 7. Настройка Конвейера (Pipeline)
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Значение HSTS по умолчанию - 30 дней.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ! ВАЖНО: Порядок вызова этих двух методов имеет значение !
app.UseAuthentication(); // <--- Включаем проверку "Кто это?" (Куки)
app.UseAuthorization();  // <--- Включаем проверку "Можно ли ему сюда?"

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();