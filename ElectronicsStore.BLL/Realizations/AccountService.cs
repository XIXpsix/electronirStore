using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Enum;
using ElectronicsStore.Domain.Response;
using ElectronicsStore.Domain.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ElectronicsStore.BLL.Realizations
{
    // C# 12: Основной конструктор
    // ВАЖНО: Тут должен быть IEmailService (интерфейс), а не EmailService (класс)
    public class AccountService(IBaseStorage<User> userRepository, IEmailService emailService) : IAccountService
    {
        // Оптимизация: static + HashData
        private static string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLower();
        }

        public async Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model)
        {
            try
            {
                var user = await userRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Name == model.Name || x.Email == model.Email);

                if (user != null)
                {
                    return new()
                    {
                        Description = "Пользователь с таким именем или email уже есть",
                    };
                }

                var random = new Random();
                var code = random.Next(100000, 999999).ToString();

                user = new User()
                {
                    Name = model.Name,
                    Role = Role.User,
                    Email = model.Email,
                    Password = HashPassword(model.Password),
                    CreatedAt = DateTime.UtcNow,
                    ConfirmationCode = code,
                    IsEmailConfirmed = false,
                    AvatarPath = "/img/w.png" // Указываем картинку по умолчанию при создании
                };

                await userRepository.Add(user);

                await emailService.SendEmailAsync(user.Email, "Код подтверждения регистрации",
                    $"<h3>Добро пожаловать в ElectronicsHub!</h3><p>Ваш код подтверждения: <b>{code}</b></p>");

                return new()
                {
                    Description = "На вашу почту отправлен код подтверждения",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<ClaimsIdentity>> ConfirmEmail(string email, string code)
        {
            try
            {
                var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Email == email);

                if (user is null)
                {
                    return new()
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.UserNotFound
                    };
                }

                if (user.ConfirmationCode != code)
                {
                    return new()
                    {
                        Description = "Неверный код подтверждения",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                user.IsEmailConfirmed = true;
                user.ConfirmationCode = "";

                await userRepository.Update(user);

                var result = Authenticate(user);

                return new()
                {
                    Data = result,
                    Description = "Почта успешно подтверждена!",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model)
        {
            try
            {
                var user = await userRepository.GetAll().FirstOrDefaultAsync(x => x.Email == model.Email);

                if (user is null)
                {
                    return new()
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.UserNotFound
                    };
                }

                if (user.Password != HashPassword(model.Password))
                {
                    return new()
                    {
                        Description = "Неверный пароль",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                if (!user.IsEmailConfirmed)
                {
                    return new()
                    {
                        Description = "Ваша почта не подтверждена. Проверьте входящие.",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                var result = Authenticate(user);

                return new()
                {
                    Data = result,
                    StatusCode = StatusCode.OK,
                    Description = "Успешный вход"
                };
            }
            catch (Exception ex)
            {
                return new()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        private static ClaimsIdentity Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimsIdentity.DefaultNameClaimType, user.Name ?? string.Empty),
                new(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString()),
                new("Id", user.Id.ToString()),
                new("AvatarPath", user.AvatarPath ?? "/img/w.png")
            };
            return new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}