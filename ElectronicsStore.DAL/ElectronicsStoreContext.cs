using ElectronicsStore.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;


namespace ElectronicsStore.DAL
{
    // Класс ElectronicsStoreContext наследует DbContext и представляет сессию
    // взаимодействия с базой данных.
    public class ElectronicsStoreContext : DbContext
    {
        public DbSet<Category> Categories { get; set; } = null!;
        // Набор данных (таблица) для товаров
        public DbSet<Product> Products { get; set; } = null!;
        // Набор данных (таблица) для отзывов
        public DbSet<Review> Reviews { get; set; } = null!;

        // Конструктор, принимающий параметры конфигурации
        public ElectronicsStoreContext(DbContextOptions<ElectronicsStoreContext> options) : base(options)
        {
        }

        // Метод для настройки связей и инициализации данных (Seed Data)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
                   CategoryId = 1 // Смартфоны
                },
                new Product
                {
                    Id = 2,
                    Name = "MacBook Pro M3",
                    Price = 189999.00m,
                    Description = "Мощный ноутбук для профессионалов.",
                    CategoryId = 2 // Ноутбуки
                },
                new Product
                {
                    Id = 3,
                    Name = "Зарядное устройство 65W",
                    Price = 2999.00m,
                    Description = "Компактное и быстрое зарядное устройство.",
                    CategoryId = 3 // Аксессуары
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