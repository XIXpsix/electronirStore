using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Enum;
using ElectronicsStore.Domain.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Realizations
{
    public class OrderService : IOrderService
    {
        private readonly IBaseStorage<User> _userRepository;
        private readonly IBaseStorage<Order> _orderRepository;
        private readonly IBaseStorage<Cart> _cartRepository;
        private readonly IBaseStorage<CartItem> _cartItemRepository;

        public OrderService(IBaseStorage<User> userRepository,
                            IBaseStorage<Order> orderRepository,
                            IBaseStorage<Cart> cartRepository,
                            IBaseStorage<CartItem> cartItemRepository)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
        }

        public async Task<IBaseResponse<Order>> CreateOrder(string userName, string address)
        {
            try
            {
                // 1. Ищем пользователя
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Name == userName);
                if (user == null)
                {
                    return new BaseResponse<Order>() { Description = "Пользователь не найден", StatusCode = StatusCode.UserNotFound };
                }

                // 2. Ищем его корзину с товарами
                var cart = await _cartRepository.GetAll()
                    .Include(x => x.Items)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);

                if (cart == null || !cart.Items.Any())
                {
                    return new BaseResponse<Order>() { Description = "Корзина пуста", StatusCode = StatusCode.InternalServerError };
                }

                // 3. Создаем заказ
                var order = new Order
                {
                    UserId = user.Id,
                    Address = address,
                    DateCreated = DateTime.UtcNow,
                    TotalPrice = cart.Items.Sum(x => x.Product.Price)
                };

                await _orderRepository.Add(order);

                // 4. Очищаем корзину (удаляем записи CartItem)
                foreach (var item in cart.Items)
                {
                    await _cartItemRepository.Delete(item);
                }

                return new BaseResponse<Order>() { Data = order, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Order>() { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }
    }
}