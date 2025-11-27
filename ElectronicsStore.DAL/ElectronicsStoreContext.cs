using ElectronicsStore.Domain;
using ElectronicsStore.Domain.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsStore.DAL
{
    public class ElectronicsStoreContext : IdentityDbContext<ApplicationUser>
    {
        public ElectronicsStoreContext(DbContextOptions<ElectronicsStoreContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Категории
            var catSmartphones = new Category { Id = 1, Name = "Смартфоны" };
            var catLaptops = new Category { Id = 2, Name = "Ноутбуки" };
            var catAccessories = new Category { Id = 3, Name = "Аксессуары" };

            modelBuilder.Entity<Category>().HasData(catSmartphones, catLaptops, catAccessories);

            // 2. Товары
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "iPhone 15 Pro",
                    Description = "Флагманский смартфон Apple с титановым корпусом.",
                    Price = 120000m,
                    ImagePath = "/img/iphone15.jpg",
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "MacBook Pro 16",
                    Description = "Супермощный ноутбук на чипе M3 Max.",
                    Price = 350000m,
                    ImagePath = "/img/macbook.jpg",
                    CategoryId = 2
                },
                new Product
                {
                    Id = 3,
                    Name = "AirPods Pro 2",
                    Description = "Лучшие наушники с шумоподавлением.",
                    Price = 25000m,
                    ImagePath = "/img/airpods.jpg",
                    CategoryId = 3
                }
            );
        }
    }
}