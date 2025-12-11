using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<IBaseResponse<IEnumerable<Category>>> GetCategories();

        Task<IBaseResponse<Category>> GetCategoryById(int id);
    }
}