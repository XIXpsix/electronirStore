using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Enum;
using ElectronicsStore.Domain.Response;
using ElectronicsStore.Domain.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Realizations
{
    public class AccountService : IAccountService
    {
        private readonly IBaseStorage<User> _userRepository;
        private readonly EmailService _emailService; // Добавляем сервис для отправки писем

        public AccountService(IBaseStorage<User> userRepository, EmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        // Хеширование
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        public async Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model)
        {
            try
            {
                // Проверяем, есть ли уже такой пользователь (по имени или email)
                var user = await _userRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Name == model.Name || x.Email == model.Email);

                if (user != null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь с таким именем или email уже есть",
                    };
                }

                // Генерируем случайный 6-значный код
                var random = new Random();
                var code = random.Next(100000, 999999).ToString();

                user = new User()
                {
                    Name = model.Name,
                    Role = Role.User,
                    Email = model.Email,
                    Password = HashPassword(model.Password), // Хешируем пароль
                    CreatedAt = DateTime.UtcNow,

                    // Сохраняем код и ставим статус "Не подтвержден"
                    ConfirmationCode = code,
                    IsEmailConfirmed = false
                };

                await _userRepository.Add(user);

                // Отправляем письмо с кодом
                await _emailService.SendEmailAsync(user.Email, "Код подтверждения регистрации",
                    $"<h3>Добро пожаловать в ElectronicsHub!</h3><p>Ваш код подтверждения: <b>{code}</b></p>");

                // Возвращаем ответ. Обрати внимание, Data = null, так как мы еще не входим в систему
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = "На вашу почту отправлен код подтверждения",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        // Новый метод для подтверждения почты
        public async Task<BaseResponse<ClaimsIdentity>> ConfirmEmail(string email, string code)
        {
            try
            {
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Email == email);

                if (user == null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.UserNotFound
                    };
                }

                // Проверяем код
                if (user.ConfirmationCode != code)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Неверный код подтверждения",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                // Активируем аккаунт
                user.IsEmailConfirmed = true;
                user.ConfirmationCode = ""; // Сбрасываем код

                await _userRepository.Update(user);

                // Сразу авторизуем пользователя
                var result = Authenticate(user);

                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
                    Description = "Почта успешно подтверждена!",
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ClaimsIdentity>()
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
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Email == model.Email);

                if (user == null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь не найден",
                        StatusCode = StatusCode.UserNotFound
                    };
                }

                // --- ВАЖНО: Проверка хеша пароля (а не простого текста) ---
                if (user.Password != HashPassword(model.Password))
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Неверный пароль",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                if (!user.IsEmailConfirmed)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Ваша почта не подтверждена. Проверьте входящие.",
                        StatusCode = StatusCode.InternalServerError
                    };
                }

                var result = Authenticate(user);

                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
                    StatusCode = StatusCode.OK,
                    Description = "Успешный вход"
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = ex.Message,
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        private ClaimsIdentity Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };
            return new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}