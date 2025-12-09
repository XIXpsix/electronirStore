using ElectronicsStore.DAL;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.DAL.Repositories;
using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.BLL.Realizations;
using ElectronicsStore.Domain.Entity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Подключение к базе данных PostgreSQL
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Регистрация Репозиториев (DAL) - Универсальный доступ к данным
builder.Services.AddScoped<IBaseStorage<Category>, BaseStorage<Category>>();
builder.Services.AddScoped<IBaseStorage<Product>, BaseStorage<Product>>();

// 3. Регистрация Сервисов (BLL) - Бизнес-логика
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

// 4. Добавление MVC контроллеров
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Настройка конвейера запросов
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