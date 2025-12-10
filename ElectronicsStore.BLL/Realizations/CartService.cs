using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Response;
using ElectronicsStore.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ElectronicsStore.BLL.Realizations
{
    public class CartService : ICartService
    {
        private readonly IBaseStorage<CartItem> _cartRepository;

        public CartService(IBaseStorage<CartItem> cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<BaseResponse<CartItem>> AddToCart(Guid userId, int productId)
        {
            try
            {
                var cartItem = await _cartRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

                if (cartItem != null)
                {
                    cartItem.Quantity++;
                    await _cartRepository.Update(cartItem);
                }
                else
                {
                    cartItem = new CartItem
                    {
                        UserId = userId,
                        ProductId = productId,
                        Quantity = 1,
                        DateCreated = DateTime.UtcNow
                    };
                    await _cartRepository.Add(cartItem);
                }

                return new BaseResponse<CartItem> { StatusCode = StatusCode.OK, Description = "Товар добавлен в корзину" };
            }
            catch (Exception ex)
            {
                return new BaseResponse<CartItem> { StatusCode = StatusCode.InternalServerError, Description = $"Ошибка добавления в корзину: {ex.Message}" };
            }
        }

        public async Task<BaseResponse<List<CartItem>>> GetUserCart(Guid userId)
        {
            try
            {
                // Загружаем элементы корзины, включая данные о товаре 
                var cart = await _cartRepository.GetAll()
                    .Where(x => x.UserId == userId)
                    .Include(x => x.Product)
                    .ToListAsync();

                return new BaseResponse<List<CartItem>>
                {
                    Data = cart,
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<CartItem>> { StatusCode = StatusCode.InternalServerError, Description = ex.Message };
            }
        }

        public async Task<BaseResponse<bool>> RemoveFromCart(Guid userId, int productId)
        {
            try
            {
                var item = await _cartRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

                if (item == null)
                {
                    return new BaseResponse<bool> { StatusCode = StatusCode.NotFound, Description = "Товар не найден в корзине" };
                }

                await _cartRepository.Delete(item);
                return new BaseResponse<bool> { StatusCode = StatusCode.OK, Data = true, Description = "Товар удален из корзины" };
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool> { StatusCode = StatusCode.InternalServerError, Description = ex.Message };
            }
        }
    }
}