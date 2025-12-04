using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ElectronicsStore.DAL.Repositories
{
    public class ProductImageStorage : IBaseStorage<ProductImage>
    {
        private readonly ElectronicsStoreContext _db;

        public ProductImageStorage(ElectronicsStoreContext db)
        {
            _db = db;
        }

        public async Task Add(ProductImage entity)
        {
            await _db.ProductImages.AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public void Delete(ProductImage entity)
        {
            _db.ProductImages.Remove(entity);
            _db.SaveChanges();
        }

        public IQueryable<ProductImage> GetAll()
        {
            return _db.ProductImages;
        }

        public async Task<ProductImage> Update(ProductImage entity)
        {
            _db.ProductImages.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}