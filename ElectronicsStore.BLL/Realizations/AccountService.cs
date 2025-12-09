using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.DAL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.Enum;
using ElectronicsStore.Domain.Response;
using ElectronicsStore.Domain.ViewModels; // <--- Важный using
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Realizations
{
    public class AccountService : IAccountService
    {
        private readonly IBaseStorage<User> _userRepository;

        public AccountService(IBaseStorage<User> userRepository)
        {
            _userRepository = userRepository;
        }

        // Метод Регистрации
        public async Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model)
        {
            try
            {
                // Ищем по Name
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Name == model.Name);
                if (user != null)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь с таким именем уже есть",
                    };
                }

                user = new User()
                {
                    Name = model.Name, // Используем Name
                    Role = Role.User,  // Присваиваем Enum (0 - User)
                    Email = model.Email,
                    Password = model.Password, // В реальном проекте здесь нужен хеш!
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.Add(user);

                var result = Authenticate(user);

                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
                    Description = "Пользователь успешно зарегистрирован",
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

        // Метод Входа (Login)
        public async Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model)
        {
            try
            {
                // Ищем по Name
                // Ищем пользователя по Email, так как в модели теперь Email
                // Исправленная строка: ищем по Email
                // Исправленная строка: ищем по Email
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Email == model.Email);
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Пользователь не найден"
                    };
                }

                // Проверка пароля (в реальном проекте сверять хеши)
                if (user.Password != model.Password)
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Неверный пароль"
                    };
                }

                var result = Authenticate(user);

                return new BaseResponse<ClaimsIdentity>()
                {
                    Data = result,
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

        private static object GetName(LoginViewModel model)
        {
            return model.Name;
        }

        // Вспомогательный метод для создания Claims
        private ClaimsIdentity Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                // Преобразуем Enum Role в строку для Claims
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };

            return new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}