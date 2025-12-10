using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Enum;
using ElectronicsStore.Domain.Filters;
using ElectronicsStore.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using ElectronicsStore.Domain.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Realizations
{
    public class ProductService(
        IBaseStorage<Product> productStorage,
        IBaseStorage<ProductImage> productImageStorage) : IProductService
    {
        public async Task<IBaseResponse<List<Product>>> GetProductsByFilter(ProductFilter filter)
        {
            try
            {
                // Явно указываем тип IQueryable<Product>
                IQueryable<Product> query = productStorage.GetAll().Include(x => x.Category);

                if (filter.CategoryId > 0)
                {
                    query = query.Where(x => x.CategoryId == filter.CategoryId);
                }

                if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                    var search = filter.Name.ToLower();
                    query = query.Where(x => x.Name.ToLower().Contains(search));
                }

                if (filter.MinPrice > 0) query = query.Where(x => x.Price >= filter.MinPrice);
                if (filter.MaxPrice > 0) query = query.Where(x => x.Price <= filter.MaxPrice);

                query = filter.SortType switch
                {
                    "price_asc" => query.OrderBy(x => x.Price),
                    "price_desc" => query.OrderByDescending(x => x.Price),
                    "name_asc" => query.OrderBy(x => x.Name),
                    _ => query.OrderBy(x => x.Id)
                };

                var products = await query.ToListAsync();

                return new BaseResponse<List<Product>> { Data = products, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Product>> { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<List<Product>>> GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = await productStorage.GetAll()
                    .Include(x => x.Category)
                    .Where(x => x.CategoryId == categoryId)
                    .ToListAsync();

                return new BaseResponse<List<Product>> { Data = products, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Product>> { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var products = await productStorage.GetAll()
                    .Include(x => x.Category)
                    .ToListAsync();

                return new BaseResponse<IEnumerable<Product>> { Data = products, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<Product>> { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<Product>> GetProduct(int id)
        {
            try
            {
                var product = await productStorage.GetAll()
                    .Include(x => x.Category)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (product == null)
                    return new BaseResponse<Product> { StatusCode = StatusCode.ProductNotFound, Description = "Товар не найден" };

                return new BaseResponse<Product> { Data = product, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Product> { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<List<string>>> GetImagesByProductId(int id)
        {
            try
            {
                // ИСПРАВЛЕНИЕ ЗДЕСЬ:
                // 1. Проверяем, что картинка не null (Where)
                // 2. Используем ! (null-forgiving operator), чтобы успокоить компилятор
                var images = await productImageStorage.GetAll()
                    .Where(x => x.ProductId == id && x.ImagePath != null)
                    .Select(x => x.ImagePath!)
                    .ToListAsync();

                return new BaseResponse<List<string>> { Data = images, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<string>> { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }
    }
}