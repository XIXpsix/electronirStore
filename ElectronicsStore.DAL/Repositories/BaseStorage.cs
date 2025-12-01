using ElectronicsStore.DAL.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicsStore.DAL.Repositories
{
    public class BaseStorage<T> : IBaseStorage<T> where T : class
    {
        private readonly ElectronicsStoreContext _db;

        public BaseStorage(ElectronicsStoreContext db)
        {
            _db = db;
        }

        public async Task Add(T entity)
        {
            await _db.Set<T>().AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task Delete(T entity)
        {
            _db.Set<T>().Remove(entity);
            await _db.SaveChangesAsync();
        }

        public IQueryable<T> GetAll()
        {
            return _db.Set<T>();
        }

        public async Task<T> Update(T entity)
        {
            _db.Set<T>().Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
    }
}