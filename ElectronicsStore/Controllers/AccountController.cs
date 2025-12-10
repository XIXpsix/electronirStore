using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace ElectronicsStore.Controllers
{
    public class AccountController(IAccountService accountService) : Controller
    {
        // --- РЕГИСТРАЦИЯ ---
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await accountService.Register(model);
                if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
                {
                    // Успех! Перенаправляем на страницу Входа
                    return RedirectToAction("Login", "Account");
                }
                // Ошибка (например, такая почта занята): показываем её на той же странице
                ModelState.AddModelError("", response.Description);
            }
            // Если что-то не так, возвращаем ту же страницу, чтобы пользователь исправил данные
            return View(model);
        }

        // --- ВХОД ---
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await accountService.Login(model);
                if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
                {
                    // Вход успешен, создаем куки
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response.Data));
                    return RedirectToAction("Index", "Home");
                }
                // Ошибка (неверный пароль): показываем её прямо на форме входа
                ModelState.AddModelError("", response.Description);
            }
            return View(model);
        }

        // --- ВЫХОД ---
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // --- GOOGLE ВХОД ---
        [HttpGet]
        public IActionResult AuthenticationGoogle()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal == null) return RedirectToAction("Login");

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            // Пытаемся найти картинку профиля
            var avatar = claims?.FirstOrDefault(c => c.Type == "picture")?.Value ??
                         claims?.FirstOrDefault(c => c.Type == "image")?.Value;

            var userModel = new User()
            {
                Name = name ?? "GoogleUser",
                Email = email,
                AvatarPath = avatar
            };

            var response = await accountService.IsCreatedAccount(userModel);

            if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK && response.Data != null)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response.Data));
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login");
        }

        // Метод подтверждения почты (если нужен)
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await accountService.ConfirmEmail(model.Email, model.Code);
                if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response.Data));
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", response.Description);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}