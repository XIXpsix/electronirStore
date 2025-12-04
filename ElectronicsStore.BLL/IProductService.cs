using ElectronicsStore.Domain;
using ElectronicsStore.Domain.Filters;
using ElectronicsStore.Domain.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Interfaces
{
    public interface IProductService
    {
        Task<IBaseResponse<Product>> GetProduct(int id);

        Task<IBaseResponse<List<Product>>> GetProductsByFilter(ProductFilter filter);
        Task<IBaseResponse<List<Product>>> GetProductsByCategory(int categoryId);
        Task<IBaseResponse<IEnumerable<Product>>> GetProducts();
    }
}