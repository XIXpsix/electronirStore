using ElectronicsStore.Domain.Entity;
using ElectronicsStore.BLL;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectronicsStore.Domain.Response;
namespace ElectronicsStore.BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<IBaseResponse<List<Category>>> GetAllCategories();
    }
}