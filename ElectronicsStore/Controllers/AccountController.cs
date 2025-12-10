using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ElectronicsStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.Register(model);

                if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
                {
                    return RedirectToAction("ConfirmEmail", "Account", new { email = model.Email });
                }
                // ИСПРАВЛЕНИЕ: Безопасное обращение к Description
                ModelState.AddModelError("", response.Description ?? "Ошибка при регистрации");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ConfirmEmail(string email)
        {
            return View(new ConfirmEmailViewModel { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.ConfirmEmail(model.Email, model.Code);

                // ИСПРАВЛЕНИЕ: Проверка Data на null уже была, но добавлена проверка Description ниже
                if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK && response.Data != null)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response.Data));
                    return RedirectToAction("Index", "Home");
                }
                // ИСПРАВЛЕНИЕ: Безопасное обращение к Description
                ModelState.AddModelError("", response.Description ?? "Неверный код или ошибка подтверждения");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.Login(model);

                // ИСПРАВЛЕНИЕ: Проверка Data на null уже была
                if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK && response.Data != null)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response.Data));
                    return RedirectToAction("Index", "Home");
                }
                // ИСПРАВЛЕНИЕ: Безопасное обращение к Description
                ModelState.AddModelError("", response.Description ?? "Неверный логин или пароль");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}