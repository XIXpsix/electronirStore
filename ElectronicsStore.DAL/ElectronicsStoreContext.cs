using ElectronicsStore.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsStore.DAL
{
    public class ElectronicsStoreContext(DbContextOptions<ElectronicsStoreContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Связь: Отзыв -> Пользователь
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь: Отзыв -> Товар
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь: Товар -> Категория
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Начальные данные для категорий
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Смартфоны",
                    Slug = "smartphones",
                    Description = "Мобильные телефоны",
                    ImagePath = "/img/category_phones.jpg"
                },
                new Category
                {
                    Id = 2,
                    Name = "Ноутбуки",
                    Slug = "laptops",
                    Description = "Лэптопы и ПК",
                    ImagePath = "/img/category_laptops.jpg"
                }
            );

            // Начальные данные для товаров
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "iPhone 15",
                    Description = "Мощный смартфон от Apple",
                    Price = 100000,
                    CategoryId = 1,
                    ImagePath = "/img/iphone15.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 2,
                    Name = "Samsung Galaxy S24",
                    Description = "Флагман на Android с отличным экраном",
                    Price = 95000,
                    CategoryId = 1,
                    ImagePath = "/img/samsung_s24.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 3,
                    Name = "Google Pixel 8",
                    Description = "Смартфон с лучшей камерой и чистым Android",
                    Price = 75000,
                    CategoryId = 1,
                    ImagePath = "/img/pixel8.jpg",
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 4,
                    Name = "MacBook Air M2",
                    Description = "Легкий, тонкий и производительный ультрабук",
                    Price = 120000,
                    CategoryId = 2,
                    ImagePath = "/img/macbook_air.jpg",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}