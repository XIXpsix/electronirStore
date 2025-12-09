using ElectronicsStore.DAL;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Security.Claims;

namespace ElectronicsStore.Controllers
{
    // Исправлено: Используем Primary Constructor (параметры прямо в имени класса)
    public class AccountController(ElectronicsStoreContext context, IMemoryCache memoryCache) : Controller
    {
        // ===================== ВХОД (LOGIN) =====================
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            // Используем LoginOrEmail (как в исправленной модели)
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Email == model.LoginOrEmail || u.Login == model.LoginOrEmail);

            if (user == null || user.Password != model.Password)
            {
                return BadRequest(new { message = "Неверный логин или пароль" });
            }

            await Authenticate(user);
            // Если ReturnUrl null, идем на главную ("/")
            return Ok(new { message = "Вход выполнен успешно!", returnUrl = model.ReturnUrl ?? "/" });
        }

        // ===================== РЕГИСТРАЦИЯ =====================
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (await context.Users.AnyAsync(u => u.Email == model.Email || u.Login == model.Login))
            {
                return BadRequest(new { message = "Пользователь уже существует" });
            }

            // Упрощенное создание рандома (Random.Shared)
            var code = Random.Shared.Next(100000, 999999).ToString();

            // === ВЫВОД КОДА В КОНСОЛЬ (Вместо почты, чтобы не падало) ===
            Debug.WriteLine($"*********************************************");
            Debug.WriteLine($" КОД ДЛЯ {model.Email}: {code}");
            Debug.WriteLine($"*********************************************");

            memoryCache.Set(model.Email, new { Model = model, Code = code }, TimeSpan.FromMinutes(10));

            return Ok(new { message = "Код отправлен", redirectUrl = Url.Action("ConfirmEmail", new { email = model.Email }) });
        }

        // ===================== ПОДТВЕРЖДЕНИЕ =====================
        [HttpGet]
        public IActionResult ConfirmEmail(string email)
        {
            return View(new ConfirmEmailViewModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (!memoryCache.TryGetValue(model.Email, out object? cacheEntry) || cacheEntry == null)
            {
                ModelState.AddModelError("", "Время истекло.");
                return View(model);
            }

            dynamic data = cacheEntry;
            string storedCode = data.Code;
            RegisterViewModel registerData = data.Model;

            if (storedCode != model.Code)
            {
                ModelState.AddModelError("Code", "Неверный код");
                return View(model);
            }

            // Создаем пользователя
            var user = new User
            {
                Id = Guid.NewGuid(),
                Login = registerData.Login,
                Email = registerData.Email,
                Password = registerData.Password,
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            memoryCache.Remove(model.Email);
            await Authenticate(user);

            return RedirectToAction("Index", "Home");
        }

        // ===================== GOOGLE / LOGOUT =====================
        [HttpGet]
        public async Task AuthenticationGoogle(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", new { returnUrl })
            });
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded) return RedirectToAction("Login");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            if (email != null)
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = email,
                        Login = email,
                        Password = "",
                        Role = "User",
                        CreatedAt = DateTime.UtcNow
                    };
                    context.Users.Add(user);
                    await context.SaveChangesAsync();
                }
                await Authenticate(user);
                return LocalRedirect(returnUrl ?? "/");
            }
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
    }
}