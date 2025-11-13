// 1. ВСЕ 'USING' ОБЯЗАТЕЛЬНО СНАРУЖИ, В САМОМ ВЕРХУ
using ElectronicsStore.BLL;
using ElectronicsStore.DAL;
using ElectronicsStore.Domain.Entity; // <-- Нужен для Identity
using Microsoft.AspNetCore.Identity; // <-- Нужен для Identity
using Microsoft.EntityFrameworkCore;

// 2. ОПРЕДЕЛЕНИЕ КЛАССА
internal class Program
{
    // 3. МЕТОД MAIN, ГДЕ ЖИВЕТ ВЕСЬ КОД
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --- 1. Настройка сервисов ---
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        // Регистрируем сервисы Identity (UserManager, SignInManager и т.д.)
        // Это чинит ошибку "No service registered"
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 4;
        })
            .AddEntityFrameworkStores<ElectronicsStoreContext>()
            .AddDefaultTokenProviders();

        // Регистрируем твой сервис
        builder.Services.AddScoped<ProductService>();

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // --- 2. Логика миграций ---
        try
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ElectronicsStoreContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }

        // --- 3. Конвейер (Pipeline) ---
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // 4. ПОРЯДОК ВАЖЕН: Аутентификация ДО Авторизации
        app.UseAuthentication(); // <-- Убедись, что это есть
        app.UseAuthorization(); // <-- У тебя уже было

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}