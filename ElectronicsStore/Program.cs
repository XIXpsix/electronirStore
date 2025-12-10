using ElectronicsStore.DAL;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.DAL.Repositories;
using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.BLL.Realizations;
using ElectronicsStore.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// 1. Подключение к базе данных PostgreSQL
builder.Services.AddDbContext<ElectronicsStoreContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Регистрация Репозиториев (DAL)
builder.Services.AddScoped<IBaseStorage<Category>, BaseStorage<Category>>();
builder.Services.AddScoped<IBaseStorage<Product>, BaseStorage<Product>>();
builder.Services.AddScoped<IBaseStorage<User>, BaseStorage<User>>();
builder.Services.AddScoped<IBaseStorage<CartItem>, BaseStorage<CartItem>>(); // Сервис корзины
builder.Services.AddScoped(typeof(IBaseStorage<>), typeof(BaseStorage<>)); // Универсальный репозиторий

// Регистрация доп. сервисов
builder.Services.AddScoped<EmailService>();

// 3. Регистрация Сервисов (BLL)
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICartService, CartService>();

// 4. Настройка аутентификации (Cookie)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
        options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- АВТОМАТИЧЕСКОЕ ДОБАВЛЕНИЕ КАТЕГОРИЙ ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ElectronicsStoreContext>();

    // Если категорий нет, создаем их с обязательными полями (Slug, ImagePath)
    if (!context.Categories.Any())
    {
        context.Categories.AddRange(
            new Category
            {
                Name = "Компьютеры и ноутбуки",
                Slug = "computers",       // Обязательное поле
                ImagePath = "/img/cat_pc.jpg" // Обязательное поле (заглушка)
            },
            new Category
            {
                Name = "Телефоны и планшеты",
                Slug = "phones",
                ImagePath = "/img/cat_phones.jpg"
            },
            new Category
            {
                Name = "Телевизоры и мониторы",
                Slug = "tv",
                ImagePath = "/img/cat_tv.jpg"
            },
            new Category
            {
                Name = "Периферия и аксессуары",
                Slug = "accessories",
                ImagePath = "/img/cat_acc.jpg"
            }
        );
        context.SaveChanges();
    }
}
// -------------------------------------------

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