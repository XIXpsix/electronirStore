using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Response;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Interfaces
{
    public interface IOrderService
    {
        Task<IBaseResponse<Order>> CreateOrder(string userName, string address);
    }
}