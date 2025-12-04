using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain;
using ElectronicsStore.Domain.Entity; // Добавлено для доступа к ProductImage
using ElectronicsStore.Domain.Enum;
using ElectronicsStore.Domain.Filters;
using ElectronicsStore.Domain.Response;
using ElectronicsStore.BLL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Realizations
{
    public class ProductService : IProductService
    {
        private readonly IBaseStorage<Product> _productStorage;
        // Добавляем хранилище для картинок
        private readonly IBaseStorage<ProductImage> _productImageStorage;

        // Обновляем конструктор: внедряем оба хранилища
        public ProductService(IBaseStorage<Product> productStorage, IBaseStorage<ProductImage> productImageStorage)
        {
            _productStorage = productStorage;
            _productImageStorage = productImageStorage;
        }

        // --- МЕТОДЫ ФИЛЬТРАЦИИ И ПОИСКА ---

        public async Task<IBaseResponse<List<Product>>> GetProductsByFilter(ProductFilter filter)
        {
            try
            {
                var query = _productStorage.GetAll();

                // Фильтр по категории
                query = query.Where(x => x.CategoryId == filter.CategoryId);

                // Фильтр по цене
                if (filter.MaxPrice > 0)
                {
                    query = query.Where(x => x.Price >= filter.MinPrice && x.Price <= filter.MaxPrice);
                }

                // Сортировка
                switch (filter.SortType)
                {
                    case "price_asc":
                        query = query.OrderBy(x => x.Price);
                        break;
                    case "price_desc":
                        query = query.OrderByDescending(x => x.Price);
                        break;
                    case "name_asc":
                        query = query.OrderBy(x => x.Name);
                        break;
                    default:
                        query = query.OrderBy(x => x.Id);
                        break;
                }

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

                if (!products.Any())
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

        // --- МЕТОДЫ ПОЛУЧЕНИЯ ОДНОГО ТОВАРА ---

        public async Task<IBaseResponse<Product>> GetProduct(int id)
        {
            try
            {
                // Важно: Include(x => x.Category) нужен, чтобы отобразить название категории
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

        // --- НОВЫЙ МЕТОД ДЛЯ ГАЛЕРЕИ (ГЛАВА 24) ---
        public async Task<IBaseResponse<List<string>>> GetImagesByProductId(int id)
        {
            try
            {
                // Ищем все картинки, привязанные к productID
                var images = await _productImageStorage.GetAll()
                    .Where(x => x.ProductId == id)
                    .Select(x => x.ImagePath) // Выбираем только пути к файлам
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