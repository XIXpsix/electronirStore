using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity; // <-- ВАЖНО: Добавили ссылку на Entity
using ElectronicsStore.Domain.Filters;
using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Enum; // Проверь, есть ли у тебя Enum в Domain
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Realizations
{
    // Используем основной конструктор (Primary Constructor)
    public class ProductService(
        IBaseStorage<Product> productStorage,
        IBaseStorage<ProductImage> productImageStorage) : IProductService
    {
        private readonly IBaseStorage<Product> _productStorage = productStorage;
        private readonly IBaseStorage<ProductImage> _productImageStorage = productImageStorage;

        public async Task<IBaseResponse<List<Product>>> GetProductsByFilter(ProductFilter filter)
        {
            try
            {
                var query = _productStorage.GetAll();

                if (filter.CategoryId != 0)
                {
                    query = query.Where(x => x.CategoryId == filter.CategoryId);
                }

                if (filter.MaxPrice > 0)
                {
                    query = query.Where(x => x.Price >= filter.MinPrice && x.Price <= filter.MaxPrice);
                }

                query = filter.SortType switch
                {
                    "price_asc" => query.OrderBy(x => x.Price),
                    "price_desc" => query.OrderByDescending(x => x.Price),
                    "name_asc" => query.OrderBy(x => x.Name),
                    _ => query.OrderBy(x => x.Id)
                };

                var products = await query.ToListAsync();

                return new BaseResponse<List<Product>>()
                {
                    Data = products,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Product>>()
                {
                    Description = $"[GetProductsByFilter] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<List<Product>>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await _productStorage.GetAll()
                    .Where(x => x.CategoryId == categoryId)
                    .ToListAsync();

                if (products.Count == 0)
                {
                    return new BaseResponse<List<Product>>()
                    {
                        Description = "Товары не найдены",
                        StatusCode = StatusCode.OK,
                        Data = new List<Product>()
                    };
                }

                return new BaseResponse<List<Product>>()
                {
                    Data = products,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Product>>()
                {
                    Description = $"[GetProductsByCategory] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var products = await _productStorage.GetAll().ToListAsync();
                return new BaseResponse<IEnumerable<Product>>()
                {
                    Data = products,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<Product>>()
                {
                    Description = $"[GetProducts] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _productStorage.GetAll()
                    .Include(x => x.Category)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (product == null)
                {
                    return new BaseResponse<Product>()
                    {
                        Description = "Товар не найден",
                        StatusCode = StatusCode.ProductNotFound
                    };
                }

                return new BaseResponse<Product>()
                {
                    Data = product,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Product>()
                {
                    Description = $"[GetProduct] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<List<string>>> GetImagesByProductId(int id)
        {
            try
            {
                var images = await _productImageStorage.GetAll()
                    .Where(x => x.ProductId == id)
                    .Select(x => x.ImagePath)
                    .ToListAsync();

                return new BaseResponse<List<string>>()
                {
                    Data = images,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<string>>()
                {
                    Description = $"[GetImagesByProductId] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}