using ElectronicsStore.Domain;
using ElectronicsStore.BLL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Interfaces
{
    // Обязательно public!
    public interface ICategoryService
    {
        Task<IBaseResponse<List<Category>>> GetAllCategories();
    }
}