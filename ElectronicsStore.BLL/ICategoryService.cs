using ElectronicsStore.Domain;
using ElectronicsStore.BLL;    

namespace ElectronicsStore.Service.Interfaces
{
    public interface ICategoryService
    {
        Task<IBaseResponse<List<Category>>> GetAllCategories();
    }
}