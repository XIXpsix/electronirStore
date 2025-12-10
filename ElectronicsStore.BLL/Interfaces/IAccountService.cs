using ElectronicsStore.BLL;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Response;
using ElectronicsStore.Domain.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Interfaces
{
    public interface IAccountService
    {
        Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model);
        Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model);
        Task<BaseResponse<ClaimsIdentity>> ConfirmEmail(string email, string code);

        [cite_start]// Метод для входа через Google [cite: 870-871]
        Task<BaseResponse<ClaimsIdentity>> IsCreatedAccount(User model);
    }
}