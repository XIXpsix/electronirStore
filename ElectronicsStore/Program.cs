using ElectronicsStore.DAL;
using ElectronicsStore.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

// Подключаем логику (BLL)
using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.BLL.Realizations;

// Подключаем базу данных (DAL)
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.DAL.Repositories;
using ElectronicsStore.Domain; // Для классов Category и Product

var builder = WebApplication.CreateBuilder(args);

// 1. Подключение к базе данных PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Настройка Identity (пользователи, пароли)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ElectronicsStoreContext>()
    .AddDefaultTokenProviders();

// 3. Настройка входа через Google
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        IConfigurationSection googleAuthNSection =
            builder.Configuration.GetSection("Authentication:Google");

        options.ClientId = googleAuthNSection["ClientId"];
        options.ClientSecret = googleAuthNSection["ClientSecret"];

        // Исправление ошибки "Correlation failed" для localhost
        options.CorrelationCookie.SameSite = SameSiteMode.Lax;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

// 4. Настройка Cookie (пути перенаправления)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/";
    options.AccessDeniedPath = "/";
    options.SlidingExpiration = true;
});

// 5. Регистрация Сервисов (Dependency Injection)

// --- Категории ---
builder.Services.AddScoped<IBaseStorage<Category>, BaseStorage<Category>>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// --- ТОВАРЫ (Новое) ---
builder.Services.AddScoped<IBaseStorage<Product>, BaseStorage<Product>>();
builder.Services.AddScoped<IProductService, ProductService>();


// 6. Подключение MVC и кэширования
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

// Настройки почты
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

var app = builder.Build();

// Настройка конвейера запросов (Pipeline)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Кто это?
app.UseAuthorization();  // Что ему можно?

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();