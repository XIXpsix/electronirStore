using ElectronicsStore.DAL;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.DAL.Repositories;
using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.BLL.Realizations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

// 1. Самая первая важная строка - создание builder
var builder = WebApplication.CreateBuilder(args);

// 2. Подключение к базе данных
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(connectionString));

// 3. Регистрация сервисов
builder.Services.AddScoped(typeof(IBaseStorage<>), typeof(BaseStorage<>));
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

// 4. Аутентификация
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

// 5. MVC и прочее
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

// 6. Сборка приложения
var app = builder.Build();

// 7. Настройка работы приложения (Pipeline)
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