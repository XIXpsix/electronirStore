using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;

namespace ElectronicsStore.DAL.Repositories
{
    public class UserStorage : BaseStorage<User>
    {
        public UserStorage(ElectronicsStoreContext db) : base(db) { }
    }
}