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
builder.Services.AddScoped<IBaseStorage<CartItem>, BaseStorage<CartItem>>(); // <-- КОРЗИНА
builder.Services.AddScoped(typeof(IBaseStorage<>), typeof(BaseStorage<>)); // Универсальный репозиторий

// Где-то в блоке регистрации сервисов (BLL)
builder.Services.AddScoped<EmailService>();

// 3. Регистрация Сервисов (BLL)
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICartService, CartService>(); // <-- СЕРВИС КОРЗИНЫ

// 4. Настройка аутентификации (Cookie) - чтобы работал вход
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
        options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
    });

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ВАЖНО: UseAuthentication должен быть ПЕРЕД UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();