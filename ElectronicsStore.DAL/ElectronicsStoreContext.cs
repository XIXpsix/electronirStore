using ElectronicsStore.Domain;
using ElectronicsStore.Domain.Entity; // <-- РЕШЕНИЕ: Добавлен using
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // <-- РЕШЕНИЕ: Изменено для Identity
using Microsoft.EntityFrameworkCore;


namespace ElectronicsStore.DAL
{
    // РЕШЕНИЕ: Контекст должен наследовать IdentityDbContext, а не DbContext
    public class ElectronicsStoreContext : IdentityDbContext<ApplicationUser> // <-- Указан ApplicationUser
    {
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        public ElectronicsStoreContext(DbContextOptions<ElectronicsStoreContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // <-- Оставить base.OnModelCreating()

            // Инициализация категорий
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Смартфоны", Slug = "smartphones" },
                new Category { Id = 2, Name = "Ноутбуки", Slug = "laptops" },
                new Category { Id = 3, Name = "Аксессуары", Slug = "accessories" }
            };

            modelBuilder.Entity<Category>().HasData(categories);

            // Инициализация товаров
            var products = new List<Product>
            {
                new Product
                {
                   Id = 1,
                   Name = "iPhone 15 Pro",
                   Price = 99999.99m,
                   Description = "Флагманский смартфон с чипом A17 Bionic.",
                   CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "MacBook Pro M3",
                    Price = 189999.00m,
                    Description = "Мощный ноутбук для профессионалов.",
                    CategoryId = 2
                },
                new Product
                {
                    Id = 3,
                    Name = "Зарядное устройство 65W",
                    Price = 2999.00m,
                    Description = "Компактное и быстрое зарядное устройство.",
                    CategoryId = 3
                }
            };

            modelBuilder.Entity<Product>().HasData(products);

            // Инициализация отзывов
            var reviews = new List<Review>
            {
                new Review
                {
                    Id = 1,
                    ProductId = 1,
                    Author = "Иван Петров",
                    Rating = 5,
                    Text = "Отличный телефон, работает невероятно быстро."
                },
                new Review
                {
                    Id = 2,
                    ProductId = 2,
                    Author = "Елена Смирнова",
                    Rating = 4,
                    Text = "Ноутбук мощный, но немного тяжеловат."
                }
            };

            modelBuilder.Entity<Review>().HasData(reviews);
        }
    }
}