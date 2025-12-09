using ElectronicsStore.Domain.Response;
using ElectronicsStore.Models; // Тут лежат LoginViewModel и RegisterViewModel
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Interfaces
{
    public interface IAccountService
    {
        Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model);
        Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model);
    }
}