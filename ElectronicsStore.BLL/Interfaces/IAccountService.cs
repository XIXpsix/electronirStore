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

        // --- ДОБАВИТЬ ЭТУ СТРОКУ: ---
        Task<BaseResponse<ClaimsIdentity>> ConfirmEmail(string email, string code);
    }
}