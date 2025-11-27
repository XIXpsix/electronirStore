using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicsStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Проверка: занят ли такой логин?
                var existingUser = await _userManager.FindByNameAsync(model.Login);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Такой логин уже занят." });
                }

                var user = new ApplicationUser
                {
                    Email = model.Email,
                    UserName = model.Login, // ✅ Сохраняем Логин как UserName
                    EmailConfirmed = true,
                    FirstName = "", // Пустые, так как мы их убрали
                    LastName = ""
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    // Возвращаем данные для авто-заполнения (если нужно)
                    return Ok(new { email = model.Email, login = model.Login });
                }
                return BadRequest(new { message = string.Join("; ", result.Errors.Select(e => e.Description)) });
            }
            return BadRequest(new { message = "Некорректные данные регистрации" });
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Сначала пытаемся найти по Email
                var user = await _userManager.FindByEmailAsync(model.LoginOrEmail);

                // 2. Если по Email не нашли, ищем по Логину (UserName)
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(model.LoginOrEmail);
                }

                if (user == null)
                {
                    return BadRequest(new { message = "Пользователь не найден." });
                }

                // 3. Проверяем пароль (используем user.UserName, так как мы его точно нашли)
                var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return Ok(new { email = user.Email, message = "Вход успешен!" });
                }
                return BadRequest(new { message = "Неверный пароль." });
            }
            return BadRequest(new { message = "Введите логин и пароль." });
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}