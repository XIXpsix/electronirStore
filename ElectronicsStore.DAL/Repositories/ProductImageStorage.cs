using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;

namespace ElectronicsStore.DAL.Repositories
{
    public class ProductImageStorage : BaseStorage<ProductImage>
    {
        public ProductImageStorage(ElectronicsStoreContext db) : base(db) { }
    }
}