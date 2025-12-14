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
    // Используем IEmailService (интерфейс)
    public class AccountService(IBaseStorage<User> userRepository, IEmailService emailService) : IAccountService
    {
        private static string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLower();
        }

        // --- МЕТОД ДЛЯ ВХОДА ЧЕРЕЗ GOOGLE ---
        public async Task<BaseResponse<ClaimsIdentity>> IsCreatedAccount(User model)
        {
            try
            {
                // ИСПРАВЛЕНО: Устранена ошибка лямбда-выражения (используется Where)
                var user = await userRepository.GetAll().Where(x => x.Email == model.Email).FirstOrDefaultAsync();
                if (user == null)
                {
                    user = new User()
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Role = Role.User,
                        Password = HashPassword("GoogleAuth_" + Guid.NewGuid().ToString()),
                        IsEmailConfirmed = true,
                        AvatarPath = model.AvatarPath ?? "/img/w.png",
                        CreatedAt = DateTime.UtcNow,
                        ConfirmationCode = "Google"
                    };
                    await userRepository.Add(user);
                }
                var result = Authenticate(user);
                return new() { Data = result, Description = "Вход через Google успешен", StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new() { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        // --- СТАНДАРТНЫЕ МЕТОДЫ ---
        public async Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model)
        {
            try
            {
                // ИСПРАВЛЕНО: Устранена ошибка лямбда-выражения (используется Where)
                var user = await userRepository.GetAll().Where(x => x.Email == model.Email).FirstOrDefaultAsync();
                if (user != null) return new() { Description = "Пользователь с таким email уже есть" };

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
                    AvatarPath = "/img/w.png"
                };

                await userRepository.Add(user);
                await emailService.SendEmailAsync(user.Email, "Код подтверждения", $"Ваш код: <b>{code}</b>");

                return new() { Description = "Код отправлен", StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new() { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model)
        {
            try
            {
                // ИСПРАВЛЕНО: Устранена ошибка лямбда-выражения (используется Where)
                var user = await userRepository.GetAll().Where(x => x.Email == model.Email).FirstOrDefaultAsync();
                if (user == null) return new() { Description = "Пользователь не найден", StatusCode = StatusCode.UserNotFound };

                if (user.Password != HashPassword(model.Password)) return new() { Description = "Неверный пароль", StatusCode = StatusCode.InternalServerError };

                if (!user.IsEmailConfirmed) return new() { Description = "Почта не подтверждена", StatusCode = StatusCode.InternalServerError };

                var result = Authenticate(user);
                return new() { Data = result, StatusCode = StatusCode.OK, Description = "Успешный вход" };
            }
            catch (Exception ex)
            {
                return new() { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        public async Task<BaseResponse<ClaimsIdentity>> ConfirmEmail(string email, string code)
        {
            try
            {
                // ИСПРАВЛЕНО: Устранена ошибка лямбда-выражения (используется Where)
                var user = await userRepository.GetAll().Where(x => x.Email == email).FirstOrDefaultAsync();
                if (user == null) return new() { Description = "Пользователь не найден" };

                if (user.ConfirmationCode != code) return new() { Description = "Неверный код" };

                user.IsEmailConfirmed = true;
                user.ConfirmationCode = "";
                await userRepository.Update(user);

                var result = Authenticate(user);
                return new() { Data = result, StatusCode = StatusCode.OK, Description = "Почта подтверждена" };
            }
            catch (Exception ex)
            {
                return new() { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        // --- НОВЫЕ МЕТОДЫ ДЛЯ ПРОФИЛЯ ---

        public async Task<BaseResponse<User>> GetUser(string? name) // ✅ Обновленная сигнатура
        {
            try
            {
                // ✅ ИСПРАВЛЕНИЕ NRT: Проверка на null/пустую строку перед использованием
                if (string.IsNullOrWhiteSpace(name))
                {
                    return new() { Description = "Имя пользователя не указано", StatusCode = StatusCode.UserNotFound };
                }

                // ИСПРАВЛЕНО: Устранена ошибка лямбда-выражения (используется Where)
                // Ищем пользователя по имени (которое в нашей системе является Name из клеймов)
                // Теперь безопасно использовать 'name' в лямбда-выражении
                var user = await userRepository.GetAll().Where(x => x.Name == name).FirstOrDefaultAsync();
                if (user == null) return new() { Description = "Пользователь не найден", StatusCode = StatusCode.UserNotFound };

                return new() { Data = user, StatusCode = StatusCode.OK };
            }
            catch (Exception ex)
            {
                return new() { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        // ИСПРАВЛЕНО: name теперь string?
        public async Task<BaseResponse<User>> EditProfile(string name, UserProfileViewModel model, string? newAvatarPath)
        {
            try
            {
                // ✅ ИСПРАВЛЕНИЕ NRT: Проверка на null/пустую строку перед использованием
                if (string.IsNullOrWhiteSpace(name))
                {
                    return new() { Description = "Имя пользователя не указано", StatusCode = StatusCode.BadRequest };
                }

                // ИСПРАВЛЕНО: Устранена ошибка лямбда-выражения (используется Where)
                var user = await userRepository.GetAll().Where(x => x.Name == name).FirstOrDefaultAsync();
                if (user == null) 
                    return new() { Description = "Пользователь не найден", StatusCode = StatusCode.UserNotFound };

                // Обновляем данные
                user.Name = model.Name ?? user.Name;
                user.Email = model.Email ?? user.Email;

                // ИСПРАВЛЕНО NRT: newAvatarPath обрабатывается правильно
                if (!string.IsNullOrEmpty(newAvatarPath))
                {
                    user.AvatarPath = newAvatarPath;
                }

                await userRepository.Update(user);

                return new() { Data = user, StatusCode = StatusCode.OK, Description = "Профиль обновлен" };
            }
            catch (Exception ex)
            {
                return new() { Description = ex.Message, StatusCode = StatusCode.InternalServerError };
            }
        }

        private static ClaimsIdentity Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                // Имя пользователя (DefaultNameClaimType) должно быть уникальным, 
                // в вашей системе это, по-видимому, Email или Name, используем Name
                new(ClaimsIdentity.DefaultNameClaimType, user.Name ?? ""),
                new(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString()),
                new("Id", user.Id.ToString()),
                new("AvatarPath", user.AvatarPath ?? "/img/w.png")
            };
            // ПРЕДУПРЕЖДЕНИЕ: "Инициализацию коллекции можно упростить"
            // Этот код уже использует современный синтаксис инициализации. 
            // Это предупреждение часто можно игнорировать.
            return new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}