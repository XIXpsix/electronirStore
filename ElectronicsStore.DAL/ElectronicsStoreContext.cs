using ElectronicsStore.Domain; // Оставляем
using ElectronicsStore.Domain.Entity;
using Microsoft.EntityFrameworkCore;


namespace ElectronicsStore.DAL
{
    public class ElectronicsStoreContext : DbContext
    {
        public ElectronicsStoreContext(DbContextOptions<ElectronicsStoreContext> options)
            : base(options)
        {
        }

        // Таблицы
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Смартфоны", Slug = "smartphones", Description = "Телефоны" },
                new Category { Id = 2, Name = "Ноутбуки", Slug = "laptops", Description = "Ноутбуки" }
            );
        }
    }
}