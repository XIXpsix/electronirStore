using ElectronicsStore.Domain.Response;
using ElectronicsStore.Domain.ViewModels; // <--- ОБЯЗАТЕЛЬНО ЭТОТ USING
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