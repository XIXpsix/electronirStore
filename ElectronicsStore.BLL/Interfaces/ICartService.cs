using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Interfaces
{
    public interface ICartService
    {
        Task<IBaseResponse<IEnumerable<CartItem>>> GetItems(string? userName);
        Task<IBaseResponse<CartItem>> AddItem(string? userName, int productId);
        Task<IBaseResponse<bool>> RemoveItem(string? userName, Guid itemId);
        Task<IBaseResponse<bool>> ClearCart(string? userName);
    }
}