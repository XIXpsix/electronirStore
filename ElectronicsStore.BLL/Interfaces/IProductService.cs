using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Filters;
using ElectronicsStore.Domain.Response;
using ElectronicsStore.Domain.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Interfaces
{
    public interface IProductService
    {
        // Существующие методы
        Task<IBaseResponse<Product>> GetProduct(int id);
        Task<IBaseResponse<List<string>>> GetImagesByProductId(int id);
        Task<IBaseResponse<List<Product>>> GetProductsByFilter(ProductFilter filter);
        Task<IBaseResponse<List<Product>>> GetProductsByCategory(int categoryId);
        Task<IBaseResponse<IEnumerable<Product>>> GetProducts();
        Task<IBaseResponse<Review>> AddReview(string? userName, int productId, string content, int rating);

        // Методы Админки
        Task<IBaseResponse<Product>> CreateProduct(ProductViewModel model);
        Task<IBaseResponse<Product>> EditProduct(ProductViewModel model);
        Task<IBaseResponse<bool>> DeleteProduct(int id);
        Task<IBaseResponse<ProductViewModel>> GetProductForEdit(int id);
        Task<IBaseResponse<bool>> AddImage(int productId, string imagePath);
        Task<IBaseResponse<bool>> DeleteImage(int imageId);
    }
}