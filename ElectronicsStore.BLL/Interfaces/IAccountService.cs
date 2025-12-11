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
        Task<BaseResponse<ClaimsIdentity>> IsCreatedAccount(User model);

        // Новые методы для профиля
        Task<BaseResponse<User>> GetUser(string? name);
        // ИСПРАВЛЕНО: newAvatarPath теперь string? для устранения NRT-предупреждения
        Task<BaseResponse<User>> EditProfile(string name, UserProfileViewModel model, string? newAvatarPath);
    }
}