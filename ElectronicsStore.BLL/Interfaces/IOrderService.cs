using ElectronicsStore.Domain.Entity; 
using ElectronicsStore.Domain.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IOrderService
{
    Task<IBaseResponse<Order>> CreateOrder(string userName, string address);

    Task<IBaseResponse<List<Order>>> GetOrders(string userName);
}