using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Enum;
using ElectronicsStore.Domain.Filters;
using ElectronicsStore.Domain.Response;
using ElectronicsStore.Domain.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Realizations
{
    public class ProductService : IProductService
    {
        private readonly IBaseStorage<Product> _productRepository;
        private readonly IBaseStorage<ProductImage> _productImageRepository;
        private readonly IBaseStorage<User> _userRepository;
        private readonly IBaseStorage<Review> _reviewRepository;
        private readonly IBaseStorage<Category> _categoryRepository;

        public ProductService(IBaseStorage<Product> productRepository,
                              IBaseStorage<ProductImage> productImageRepository,
                              IBaseStorage<User> userRepository,
                              IBaseStorage<Review> reviewRepository,
                              IBaseStorage<Category> categoryRepository)
        {
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _userRepository = userRepository;
            _reviewRepository = reviewRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IBaseResponse<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetAll()
                    .Include(x => x.Category)
                    .Include(x => x.Images)
                    .Include(x => x.Reviews!).ThenInclude(r => r.User)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (product == null)
                {
                    return new BaseResponse<Product>()
                    {
                        Description = "Товар не найден",
                        StatusCode = StatusCode.ProductNotFound
                    };
                }

                return new BaseResponse<Product>() { Data = product, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Product>() { Description = $"[GetProduct] : {ex.Message}", StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var products = await _productRepository.GetAll()
                    .Include(x => x.Category)
                    .Include(x => x.Images)
                    .ToListAsync();

                return new BaseResponse<IEnumerable<Product>>() { Data = products, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<Product>>() { Description = $"[GetProducts] : {ex.Message}", StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<List<Product>>> GetProductsByFilter(ProductFilter filter)
        {
            try
            {
                var query = _productRepository.GetAll()
                    .Include(x => x.Category)
                    .Include(x => x.Images)
                    .AsQueryable();

                // 1. Фильтр по категории
                if (filter.CategoryId > 0)
                {
                    query = query.Where(x => x.CategoryId == filter.CategoryId);
                }

                // 2. Поиск по имени (регистронезависимый)
                if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                    // Используем ToLower для надежности, хотя некоторые БД и так игнорируют регистр
                    query = query.Where(x => x.Name.ToLower().Contains(filter.Name.ToLower()));
                }

                // 3. Фильтр по цене
                if (filter.MinPrice.HasValue && filter.MinPrice.Value > 0)
                {
                    query = query.Where(x => x.Price >= filter.MinPrice.Value);
                }

                if (filter.MaxPrice.HasValue && filter.MaxPrice.Value > 0)
                {
                    query = query.Where(x => x.Price <= filter.MaxPrice.Value);
                }

                // 4. Сортировка
                query = filter.SortType switch
                {
                    "price_asc" => query.OrderBy(x => x.Price),
                    "price_desc" => query.OrderByDescending(x => x.Price),
                    "name_asc" => query.OrderBy(x => x.Name),
                    "name_desc" => query.OrderByDescending(x => x.Name),
                    _ => query.OrderByDescending(x => x.Id) // По умолчанию новые сверху
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
                var products = await _productRepository.GetAll()
                    .Include(x => x.Category)
                    .Include(x => x.Images)
                    .Where(x => x.CategoryId == categoryId)
                    .ToListAsync();

                return new BaseResponse<List<Product>> { Data = products, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Product>> { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<List<string>>> GetImagesByProductId(int id)
        {
            try
            {
                var images = await _productImageRepository.GetAll()
                    .Where(x => x.ProductId == id && x.ImagePath != null)
                    .Select(x => x.ImagePath ?? "")
                    .ToListAsync();

                return new BaseResponse<List<string>> { Data = images, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<string>> { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<Review>> AddReview(string? userName, int productId, string content, int rating)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                {
                    return new BaseResponse<Review>() { Description = "Имя пользователя не найдено", StatusCode = StatusCode.UserNotFound };
                }

                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Name == userName);
                var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == productId);

                if (user == null || product == null)
                {
                    return new BaseResponse<Review>() { Description = "Пользователь или товар не найден", StatusCode = StatusCode.UserNotFound };
                }

                var review = new Review
                {
                    UserId = user.Id,
                    ProductId = product.Id,
                    Content = content,
                    Rating = rating,
                    CreatedAt = DateTime.Now
                };

                await _reviewRepository.Add(review);

                return new BaseResponse<Review>() { Data = review, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Review>() { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<Product>> CreateProduct(ProductViewModel model)
        {
            try
            {
                var product = new Product
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    CategoryId = model.CategoryId,
                    CreatedAt = DateTime.Now
                };

                await _productRepository.Add(product);

                return new BaseResponse<Product>() { Data = product, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Product>() { Description = $"[CreateProduct] : {ex.Message}", StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<ProductViewModel>> GetProductForEdit(int id)
        {
            try
            {
                var product = await _productRepository.GetAll()
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (product == null)
                    return new BaseResponse<ProductViewModel>() { Description = "Товар не найден", StatusCode = StatusCode.ProductNotFound };

                var model = new ProductViewModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    ExistingImages = product.Images.ToList()
                };

                return new BaseResponse<ProductViewModel>() { Data = model, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ProductViewModel>() { Description = $"[GetProductForEdit] : {ex.Message}", StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<Product>> EditProduct(ProductViewModel model)
        {
            try
            {
                var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == model.Id);

                if (product == null)
                    return new BaseResponse<Product>() { Description = "Товар не найден", StatusCode = StatusCode.ProductNotFound };

                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;
                product.UpdatedAt = DateTime.Now;

                await _productRepository.Update(product);

                return new BaseResponse<Product>() { Data = product, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Product>() { Description = $"[EditProduct] : {ex.Message}", StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<bool>> DeleteProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetAll()
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (product == null)
                    return new BaseResponse<bool>() { Description = "Товар не найден", StatusCode = StatusCode.ProductNotFound };

                if (product.Images != null)
                {
                    foreach (var image in product.Images)
                    {
                        await _productImageRepository.Delete(image);
                    }
                }

                await _productRepository.Delete(product);

                return new BaseResponse<bool>() { Data = true, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>() { Description = $"[DeleteProduct] : {ex.Message}", StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<bool>> AddImage(int productId, string imagePath)
        {
            try
            {
                var productImage = new ProductImage
                {
                    ProductId = productId,
                    ImagePath = imagePath
                };
                await _productImageRepository.Add(productImage);
                return new BaseResponse<bool> { Data = true, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool> { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<bool>> DeleteImage(int imageId)
        {
            try
            {
                var image = await _productImageRepository.GetAll().FirstOrDefaultAsync(x => x.Id == imageId);
                if (image == null) return new BaseResponse<bool> { Description = "Изображение не найдено" };

                await _productImageRepository.Delete(image);
                return new BaseResponse<bool> { Data = true, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool> { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }
    }
}