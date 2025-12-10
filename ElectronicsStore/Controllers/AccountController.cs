using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
// Псевдоним для статуса
using EnumStatusCode = ElectronicsStore.Domain.Enum.StatusCode;

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

                if (response.StatusCode == EnumStatusCode.OK)
                { 
                    return RedirectToAction("ConfirmEmail", "Account", new { email = model.Email });
                }
                ModelState.AddModelError("", response.Description);
            }
            return View(model);
        }

        // --- НОВЫЕ МЕТОДЫ ДЛЯ ПОДТВЕРЖДЕНИЯ ---

        [HttpGet]
        public IActionResult ConfirmEmail(string email)
        {
            return View(new ConfirmEmailViewModel { Email = email });
        }

        public IAccountService Get_accountService()
        {
            return _accountService;
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Теперь интерфейс знает этот метод, ошибки не будет
                var response = await _accountService.ConfirmEmail(model.Email, model.Code);

                if (response.StatusCode == EnumStatusCode.OK)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response.Data!));
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", response.Description);
            }
            return View(model);
        }

        // ----------------------------------------

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.Login(model);

                if (response.StatusCode == EnumStatusCode.OK && response.Data != null)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(response.Data));
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", response.Description);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Удаляем куки авторизации
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Перенаправляем на главную страницу (или на страницу входа)
            return RedirectToAction("Index", "Home");
        }
    }
}