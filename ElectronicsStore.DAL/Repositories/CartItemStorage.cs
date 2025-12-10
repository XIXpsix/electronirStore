using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;

namespace ElectronicsStore.DAL.Repositories
{
    public class CartItemStorage : BaseStorage<CartItem>
    {
        public CartItemStorage(ElectronicsStoreContext db) : base(db)
        {
        }
    }
}