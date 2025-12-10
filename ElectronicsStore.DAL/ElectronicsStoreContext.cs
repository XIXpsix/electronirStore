using ElectronicsStore.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsStore.DAL
{
    public class ElectronicsStoreContext : DbContext
    {
        public ElectronicsStoreContext(DbContextOptions<ElectronicsStoreContext> options) : base(options)
        { }
        
            public DbSet<User> Users { get; set; }
            public DbSet<Product> Products { get; set; }
            public DbSet<Category> Categories { get; set; }
            public DbSet<Review> Reviews { get; set; }
            public DbSet<ProductImage> ProductImages { get; set; }
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

                // Начальные данные
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

                modelBuilder.Entity<Product>().HasData(
    NewMethod()
                );
            }

        private static Product NewMethod() => new Product
        {
            Id = 1,
            Name = "iPhone 15",
            Description = "Мощный смартфон",
            Price = 100000,
            CategoryId = 1,
            ImagePath = "/img/iphone15.jpg"
        };
    }
}