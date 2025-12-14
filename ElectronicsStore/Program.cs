using ElectronicsStore;
using ElectronicsStore.DAL;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.DAL.Interfaces; // Добавлено для IBaseStorage
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(connectionString));

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
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty        ;
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
var services = scope.ServiceProvider;
try
{
// Получаем доступ к хранилищам
var userStorage = services.GetRequiredService<ElectronicsStore.DAL.Interfaces.IBaseStorage<ElectronicsStore.Domain.Entity.User>>();
var productStorage = services.GetRequiredService<ElectronicsStore.DAL.Interfaces.IBaseStorage<ElectronicsStore.Domain.Entity.Product>>();
var categoryStorage = services.GetRequiredService<ElectronicsStore.DAL.Interfaces.IBaseStorage<ElectronicsStore.Domain.Entity.Category>>();

// ЗАПУСКАЕМ СОЗДАНИЕ ТОВАРОВ
await ElectronicsStore.Initializer.InitializeData(userStorage, productStorage, categoryStorage);
}
catch (Exception ex)
{
var logger = services.GetRequiredService<ILogger<Program>>();
logger.LogError(ex, "Ошибка при заполнении базы данных.");
}
}

if (!app.Environment.IsDevelopment())
{
    // ... дальше старый код ...
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