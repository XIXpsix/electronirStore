using ElectronicsStore.Domain;
using ElectronicsStore.BLL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Interfaces
{
    public interface IProductService
    {
        // Метод для получения товаров конкретной категории
        Task<IBaseResponse<List<Product>>> GetProductsByCategory(int categoryId);
    }
}