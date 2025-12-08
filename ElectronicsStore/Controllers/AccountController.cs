using ElectronicsStore.DAL;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ElectronicsStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ElectronicsStoreContext _context;

        public AccountController(ElectronicsStoreContext context)
        {
            _context = context;
        }

        // --- ЛОГИН ---
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            // Ищем пользователя в БД
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.LoginOrEmail || u.Login == model.LoginOrEmail);

            if (user == null || user.Password != model.Password) // Тут лучше добавить хеширование!
            {
                return BadRequest(new { message = "Неверный логин или пароль" });
            }

            await Authenticate(user); // Ставим куки
            return Ok(new { message = "Вход выполнен" });
        }

        // --- РЕГИСТРАЦИЯ ---
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return BadRequest(new { message = "Почта занята" });
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Login = model.Login,
                Email = model.Email,
                Password = model.Password, // В реальном проекте хешируем!
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await Authenticate(user);
            return Ok(new { message = "Регистрация успешна" });
        }

        // --- GOOGLE ---
        public async Task GoogleLogin()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded) return RedirectToAction("Login");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;

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
            return RedirectToAction("Index", "Home");
        }

        // --- ВЫХОД ---
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Вспомогательный метод установки куки
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