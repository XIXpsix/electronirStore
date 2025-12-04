using ElectronicsStore.Domain;
using ElectronicsStore.Domain.Filters; // Подключаем фильтры
using ElectronicsStore.Domain.Response; // Убедись, что этот namespace верный (где лежит IBaseResponse)
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Interfaces
{
    public interface IProductService
    {
        // Метод для фильтрации (НОВЫЙ)
        Task<IBaseResponse<List<Product>>> GetProductsByFilter(ProductFilter filter);

        // Метод для получения товаров по категории (СТАРЫЙ)
        Task<IBaseResponse<List<Product>>> GetProductsByCategory(int categoryId);

        // Метод для получения всех товаров (СТАРЫЙ)
        Task<IBaseResponse<IEnumerable<Product>>> GetProducts();
    }
}