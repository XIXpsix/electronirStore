using ElectronicsStore.DAL;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ElectronicsStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ElectronicsStoreContext _context;

        public AccountController(ElectronicsStoreContext context)
        {
            _context = context;
        }

        // ===================== ВХОД (LOGIN) =====================
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            // 1. Ищем пользователя в базе (по Email или Логину)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.LoginOrEmail || u.Login == model.LoginOrEmail);

            // 2. Проверяем пароль (Внимание: тут сравнение без хеша, как у друга. Для продакшена нужен хеш!)
            if (user == null || user.Password != model.Password)
            {
                return BadRequest(new { message = "Неверный логин или пароль" });
            }

            // 3. Авторизуем (ставим куки)
            await Authenticate(user);

            return Ok(new { message = "Вход выполнен успешно!", returnUrl = model.ReturnUrl ?? "/" });
        }

        // ===================== РЕГИСТРАЦИЯ =====================
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            // 1. Проверяем, есть ли такой email
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return BadRequest(new { message = "Пользователь с такой почтой уже существует" });
            }

            // 2. Создаем нового пользователя
            var user = new User
            {
                Id = Guid.NewGuid(),
                Login = model.Login,
                Email = model.Email,
                Password = model.Password, // В методичке друга сохраняют пароль. В идеале нужен HashPasswordHelper
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 3. Сразу авторизуем
            await Authenticate(user);

            return Ok(new { message = "Регистрация успешна!" });
        }

        // ===================== GOOGLE AUTH =====================
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
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = email,
                        Login = email,
                        Password = "", // Без пароля
                        Role = "User",
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                await Authenticate(user);
                return LocalRedirect(returnUrl ?? "/");
            }
            return RedirectToAction("Login");
        }

        // ===================== ВЫХОД =====================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Вспомогательный метод
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