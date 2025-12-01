using ElectronicsStore.DAL;
using ElectronicsStore.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

// Подключаем наши сервисы из BLL
using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.BLL.Realizations;

// Подключаем DAL
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.DAL.Repositories; // Убедитесь, что BaseStorage здесь
using ElectronicsStore.Domain;

var builder = WebApplication.CreateBuilder(args);

// 1. Подключение к базе данных
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Настройка Identity
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

// 3. Настройка Google
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        IConfigurationSection googleAuthNSection =
            builder.Configuration.GetSection("Authentication:Google");

        options.ClientId = googleAuthNSection["ClientId"];
        options.ClientSecret = googleAuthNSection["ClientSecret"];

        options.CorrelationCookie.SameSite = SameSiteMode.Lax;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

// 4. Настройка Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/";
    options.AccessDeniedPath = "/";
    options.SlidingExpiration = true;
});

// 5. Регистрация Сервисов (Dependency Injection)

// Регистрируем репозиторий для Категорий
builder.Services.AddScoped<IBaseStorage<Category>, BaseStorage<Category>>();

// Регистрируем Сервис категорий
builder.Services.AddScoped<ICategoryService, CategoryService>();


// 6. MVC
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

var app = builder.Build();

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