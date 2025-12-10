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

                // ИСПРАВЛЕНИЕ: Полный путь к Enum, чтобы не путать с методом контроллера
                if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK)
                {
                    return RedirectToAction("ConfirmEmail", "Account", new { email = model.Email });
                }

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

                // ИСПРАВЛЕНИЕ: Полный путь к Enum
                if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK && response.Data != null)
                {
                    // ИСПРАВЛЕНИЕ: response.Data! (восклицательный знак), чтобы убрать предупреждение о null
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response.Data!));
                    return RedirectToAction("Index", "Home");
                }

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

                // ИСПРАВЛЕНИЕ: Полный путь к Enum
                if (response.StatusCode == ElectronicsStore.Domain.Enum.StatusCode.OK && response.Data != null)
                {
                    // ИСПРАВЛЕНИЕ: response.Data!
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response.Data!));
                    return RedirectToAction("Index", "Home");
                }

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