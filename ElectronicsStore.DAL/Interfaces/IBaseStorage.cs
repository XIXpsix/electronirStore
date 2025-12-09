using System.Linq;
using System.Threading.Tasks;

namespace ElectronicsStore.DAL.Interfaces
{
    public interface IBaseStorage<T>
    {
        Task Add(T entity);
        Task Delete(T entity);
        Task<T> Update(T entity);
        IQueryable<T> GetAll();
    }
}   