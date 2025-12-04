using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain;
using ElectronicsStore.Domain.Enum;
using ElectronicsStore.Domain.Filters; // Подключаем фильтры
using ElectronicsStore.Domain.Response; // Подключаем ответы
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

        public ProductService(IBaseStorage<Product> productStorage)
        {
            _productStorage = productStorage;
        }

        // НОВЫЙ МЕТОД: Фильтрация и сортировка
        public async Task<IBaseResponse<List<Product>>> GetProductsByFilter(ProductFilter filter)
        {
            try
            {
                // 1. Начинаем запрос ко всем товарам
                var query = _productStorage.GetAll();

                // 2. Фильтруем по категории (обязательно)
                query = query.Where(x => x.CategoryId == filter.CategoryId);

                // 3. Фильтруем по цене (если задан диапазон)
                // Проверяем, чтобы MaxPrice был больше 0, иначе считаем фильтр цены отключенным
                if (filter.MaxPrice > 0)
                {
                    query = query.Where(x => x.Price >= filter.MinPrice && x.Price <= filter.MaxPrice);
                }

                // 4. Сортировка
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
                        // Сортировка по умолчанию (например, по ID)
                        query = query.OrderBy(x => x.Id);
                        break;
                }

                // 5. Выполняем запрос к базе
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

        // СТАРЫЙ МЕТОД
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

        // СТАРЫЙ МЕТОД
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
    }
}