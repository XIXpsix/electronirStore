using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;

namespace ElectronicsStore.DAL.Repositories
{
    public class ProductStorage : BaseStorage<Product>
    {
        public ProductStorage(ElectronicsStoreContext db) : base(db) { }
    }
}