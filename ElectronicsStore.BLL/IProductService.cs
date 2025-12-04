using ElectronicsStore.Domain;
using ElectronicsStore.BLL;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Interfaces
{
    public interface IProductService
    {
        // Метод для фильтрации по категории
        Task<IBaseResponse<List<Product>>> GetProductsByCategory(int categoryId);

        // Метод для получения всех товаров (пригодится для админки или поиска)
        Task<IBaseResponse<IEnumerable<Product>>> GetProducts();
    }
}