using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;

namespace ElectronicsStore.DAL.Repositories
{
    public class CategoryStorage : BaseStorage<Category>
    {
        public CategoryStorage(ElectronicsStoreContext db) : base(db) { }
    }
}