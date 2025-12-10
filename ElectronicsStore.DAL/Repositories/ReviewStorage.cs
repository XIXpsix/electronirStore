using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;

namespace ElectronicsStore.DAL.Repositories
{
    public class ReviewStorage : BaseStorage<Review>
    {
        public ReviewStorage(ElectronicsStoreContext db) : base(db) { }
    }
}