using ElectronicsStore.DAL;
using ElectronicsStore.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Models; // Для доступа к EmailSettings

var builder = WebApplication.CreateBuilder(args);

// 1. Подключение к базе данных
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Настройка Identity (пользователи и роли)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;

    // Настройки пароля (как у вас было)
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<ElectronicsStoreContext>()
    .AddDefaultTokenProviders();

// 3. Настройка аутентификации через Google (ВСТАВЛЕНО СЮДА)
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        IConfigurationSection googleAuthNSection =
            builder.Configuration.GetSection("Authentication:Google");

        // Берет ID и Секрет из secrets.json или appsettings.json
        options.ClientId = googleAuthNSection["ClientId"];
        options.ClientSecret = googleAuthNSection["ClientSecret"];
    });

// 4. Настройка Cookie (пути для входа)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/";
    options.AccessDeniedPath = "/";
    options.SlidingExpiration = true;
});

// 5. Подключение MVC и других сервисов
builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

// Подключаем настройки почты из appsettings.json
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

var app = builder.Build();

// Настройка конвейера обработки запросов (Pipeline)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Аутентификация (кто ты?)
app.UseAuthorization();  // Авторизация (что тебе можно?)

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();