using ElectronicsStore.BLL;
using ElectronicsStore.DAL;
using Microsoft.EntityFrameworkCore;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --- 1. Настройка сервисов (до builder.Build()) ---

        // Получаем строку подключения. ИСПРАВЛЕНИЕ: CS0103
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        // Регистрируем сервис без интерфейса. ИСПРАВЛЕНИЕ: CS0311
        builder.Services.AddScoped<ProductService>();

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // --- 2. Выполнение логики после builder.Build() (Применение миграций) ---

        try
        {
            // Получаем контекст в правильной области видимости. ИСПРАВЛЕНИЕ: CS0103
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<ElectronicsStoreContext>();

            // Применяем ожидающие миграции
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}