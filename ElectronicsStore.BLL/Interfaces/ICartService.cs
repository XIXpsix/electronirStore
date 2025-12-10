using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Interfaces
{
    public interface ICartService
    {
        Task<BaseResponse<CartItem>> AddToCart(Guid userId, int productId);
        Task<BaseResponse<bool>> RemoveFromCart(Guid userId, int productId);
        Task<BaseResponse<List<CartItem>>> GetUserCart(Guid userId);
    }
}