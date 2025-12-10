using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicsStore.DAL.Repositories
{
    public class CartStorage : BaseStorage<Cart>
    {
        public CartStorage(ElectronicsStoreContext db) : base(db)
        {
        }
    }
}