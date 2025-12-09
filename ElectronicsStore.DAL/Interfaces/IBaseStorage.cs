using System.Linq;
using System.Threading.Tasks;

namespace ElectronicsStore.DAL.Interfaces
{
    public interface IBaseStorage<T>
    {
        Task Add(T entity);
        Task Delete(T entity);
        Task<T> Get(int id);
        IQueryable<T> GetAll();
        Task<T> Update(T entity);
    }
}