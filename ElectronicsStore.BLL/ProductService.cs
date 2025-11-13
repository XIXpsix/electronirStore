using ElectronicsStore.DAL;
using ElectronicsStore.Domain;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Enum;
using ElectronicsStore.Domain.Response;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsStore.BLL
{
    public class ProductService(ElectronicsStoreContext db) : IProductService
    {
        public async Task<IBaseResponse<IEnumerable<Product>>> GetProducts()
        {
            var baseResponse = new BaseResponse<IEnumerable<Product>>();
            try
            {
                var products = await db.Products.ToListAsync();
                if (products.Count == 0)
                {
                    // Теперь мы должны явно задать Description
                    return new BaseResponse<IEnumerable<Product>>
                    {
                        Description = "Найдено 0 элементов", // <-- ОБЯЗАТЕЛЬНОЕ ПОЛЕ
                        StatusCode = StatusCode.ProductNotFound
                    };
                }

                baseResponse.Data = products;
                baseResponse.StatusCode = StatusCode.OK;
                baseResponse.Description = "Успешно"; // <-- ОБЯЗАТЕЛЬНОЕ ПОЛЕ

                return baseResponse;
            }
            catch (Exception ex)
            {
                // И здесь задаем Description
                return new BaseResponse<IEnumerable<Product>>()
                {
                    Description = $"[GetProducts]: {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}