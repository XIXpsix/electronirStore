using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Enum;
using ElectronicsStore.Domain.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Realizations
{
    public class CartService : ICartService
    {
        private readonly IBaseStorage<Cart> _cartRepository;
        private readonly IBaseStorage<CartItem> _cartItemRepository;
        private readonly IBaseStorage<User> _userRepository;
        private readonly IBaseStorage<Product> _productRepository;

        public CartService(IBaseStorage<Cart> cartRepository,
                           IBaseStorage<CartItem> cartItemRepository,
                           IBaseStorage<User> userRepository,
                           IBaseStorage<Product> productRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        public async Task<IBaseResponse<IEnumerable<CartItem>>> GetItems(string? userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                    return new BaseResponse<IEnumerable<CartItem>> { StatusCode = StatusCode.UserNotFound };

                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Name == userName);
                if (user == null)
                {
                    return new BaseResponse<IEnumerable<CartItem>> { Description = "Пользователь не найден", StatusCode = StatusCode.UserNotFound };
                }

                var cart = await _cartRepository.GetAll()
                    .Include(x => x.Items)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (cart == null)
                {
                    return new BaseResponse<IEnumerable<CartItem>> { Data = new List<CartItem>(), StatusCode = StatusCode.OK };
                }

                return new BaseResponse<IEnumerable<CartItem>> { Data = cart.Items, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<CartItem>> { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<CartItem>> AddItem(string? userName, int productId)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                    return new BaseResponse<CartItem> { StatusCode = StatusCode.UserNotFound };

                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Name == userName);
                if (user == null) return new BaseResponse<CartItem> { StatusCode = StatusCode.UserNotFound };

                var cart = await _cartRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (cart == null)
                {
                    cart = new Cart { UserId = user.Id, Items = new List<CartItem>() };
                    await _cartRepository.Add(cart);
                }

                var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == productId);
                if (product == null) return new BaseResponse<CartItem> { StatusCode = StatusCode.ProductNotFound };

                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = product.Id
                };

                await _cartItemRepository.Add(cartItem);

                return new BaseResponse<CartItem> { Data = cartItem, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CartItem> { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<bool>> RemoveItem(string? userName, Guid itemId)
        {
            try
            {
                if (string.IsNullOrEmpty(userName)) return new BaseResponse<bool> { StatusCode = StatusCode.UserNotFound };

                var item = await _cartItemRepository.GetAll()
                    .Include(x => x.Cart).ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == itemId);

                // Проверяем, что товар существует и принадлежит пользователю
                if (item == null || item.Cart.User.Name != userName)
                {
                    return new BaseResponse<bool> { Description = "Ошибка доступа", StatusCode = StatusCode.InternalServerError };
                }

                await _cartItemRepository.Delete(item);
                return new BaseResponse<bool> { Data = true, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool> { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<IBaseResponse<bool>> ClearCart(string? userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName)) return new BaseResponse<bool> { StatusCode = StatusCode.UserNotFound };

                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Name == userName);
                if (user == null) return new BaseResponse<bool> { StatusCode = StatusCode.UserNotFound };

                var cart = await _cartRepository.GetAll()
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (cart != null && cart.Items.Any())
                {
                    foreach (var item in cart.Items)
                    {
                        await _cartItemRepository.Delete(item);
                    }
                }

                return new BaseResponse<bool> { Data = true, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool> { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }
    }
}