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
                    // После успешной регистрации перенаправляем на Вход
                    return RedirectToAction("Login", "Account");
                }
                // Если ошибка (напр. такой email есть), добавляем её на форму
                ModelState.AddModelError("", response.Description);
            }
            // Возвращаем ту же страницу с ошибками
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
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response.Data));
                    return RedirectToAction("Index", "Home");
                }
                // ОШИБКА: Добавляем описание ошибки (Неверный пароль) в модель, чтобы показать на странице
                ModelState.AddModelError("", response.Description);
            }
            return View(model);
        }

        // --- ПОДТВЕРЖДЕНИЕ ПОЧТЫ (Если используется) ---
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
            // Если была ошибка, лучше вернуть View, но так как это обычно AJAX или отдельная форма,
            // оставим Redirect или View по твоей логике. Для простоты вернем на главную с ошибкой.
            return RedirectToAction("Index", "Home");
        }

        // --- ВЫХОД ---
        [HttpPost] // Лучше использовать HttpPost для выхода ради безопасности
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Для удобства можно добавить и GET версию, если ссылка в меню обычная
        [HttpGet]
        public async Task<IActionResult> LogoutGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // --- GOOGLE AUTHENTICATION ---

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

            // Если результат пустой (ошибка со стороны гугл или отмена), возвращаем на логин
            if (result?.Principal == null) return RedirectToAction("Login");

            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            // Иногда картинка приходит как "picture", иногда как URI claim. Пробуем найти.
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
    }
}