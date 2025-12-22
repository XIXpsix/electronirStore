using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Domain.ViewModels;
using ElectronicsStore.Domain.Enum;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
// Для явного указания StatusCode

// ИСПРАВЛЕНО: Предупреждение "Использовать основной конструктор" уже устранено
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
                if (response.StatusCode == Domain.Enum.StatusCode.OK)
                {
                    // УСПЕХ: Возвращаем JSON с адресом для перехода (на ввод кода)
                    return Json(new
                    {
                        isValid = true,
                        redirectUrl = Url.Action("ConfirmEmail", "Account", new { email = model.Email })
                    });
                }
                // ОШИБКА СЕРВИСА (например, почта занята)
                // ИСПРАВЛЕНО NRT: Добавлен ?? для response.Description
                return Json(new { isValid = false, description = response.Description ?? "Неизвестная ошибка регистрации" });
            }

            // ОШИБКА ВАЛИДАЦИИ (пустые поля и т.д.)
            // ИСПРАВЛЕНО NRT: Убеждаемся, что ErrorMessage не null перед Join
            var errorMsg = string.Join("\n", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).Where(e => e != null));
            return Json(new { isValid = false, description = errorMsg });
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
                // ИСПРАВЛЕНО: Добавлена явная проверка Data != null для устранения NRT-предупреждения
                if (response.StatusCode == Domain.Enum.StatusCode.OK && response.Data != null)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response.Data));
                    // УСПЕХ: Возвращаем JSON с адресом главной страницы
                    return Json(new { isValid = true, redirectUrl = Url.Action("Index", "Home") });
                }
                // ИСПРАВЛЕНО NRT: Добавлен ?? для response.Description
                return Json(new { isValid = false, description = response.Description ?? "Неизвестная ошибка входа" });
            }
            // ИСПРАВЛЕНО NRT: Убеждаемся, что ErrorMessage не null перед Join
            var errorMsg = string.Join("\n", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).Where(e => e != null));
            return Json(new { isValid = false, description = errorMsg });
        }

        // --- ПОДТВЕРЖДЕНИЕ ПОЧТЫ ---
        [HttpGet]
        public IActionResult ConfirmEmail(string? email)
        {
            return View(new ConfirmEmailViewModel { Email = email ?? string.Empty });
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await accountService.ConfirmEmail(model.Email, model.Code);
                // ИСПРАВЛЕНО: Добавлена явная проверка Data != null для устранения NRT-предупреждения
                if (response.StatusCode == Domain.Enum.StatusCode.OK && response.Data != null)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response.Data));
                    return RedirectToAction("Index", "Home");
                }
                // ИСПРАВЛЕНО: response.Description уже обрабатывается оператором ??, что решает NRT-предупреждение
                ModelState.AddModelError("", response.Description ?? "Неизвестная ошибка подтверждения.");
            }
            return View(model);
        }

        // --- ВЫХОД ---
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // --- GOOGLE ---
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

            if (result?.Principal == null)
            {
                return RedirectToAction("Login");
            }

            // --- БЕЗОПАСНЫЙ ДОСТУП К Principal ---
            // Теперь мы уверены, что result.Principal не null, и можем работать с ним напрямую.
            var principal = result.Principal;

            var claims = principal.Identities.FirstOrDefault()?.Claims;
            // ИСПРАВЛЕНО NRT: Все переменные name/email/avatar могут быть null
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var avatar = claims?.FirstOrDefault(c => c.Type == "picture")?.Value ?? claims?.FirstOrDefault(c => c.Type == "image")?.Value;

            // ИСПРАВЛЕНО NRT: Использован оператор ?? для гарантированной передачи не-null значений
            var userModel = new User()
            {
                Name = name ?? "GoogleUser",
                Email = email ?? string.Empty,
                AvatarPath = avatar ?? "/img/default-user.png"
            };
            var response = await accountService.IsCreatedAccount(userModel);

            if (response.StatusCode == Domain.Enum.StatusCode.OK && response.Data != null)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response.Data));
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login");
        }
    }
}