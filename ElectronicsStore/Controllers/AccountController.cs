using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Identity; // <-- Это "движок"
using Microsoft.AspNetCore.Mvc;

namespace ElectronicsStore.Controllers
{
    // ✅ Мы "просим" C# дать нам доступ к UserManager и SignInManager
    public class AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager) : Controller
    {
        // Сохраняем их в приватные поля, чтобы использовать в методах
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;


        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            // Проверяем валидацию (из RegisterViewModel.cs)
            if (ModelState.IsValid)
            {
                // Создаем нового пользователя на основе данных из формы
                var user = new ApplicationUser
                {
                    UserName = model.Email, // Логин = Email
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                // ✅ ПЫТАЕМСЯ СОЗДАТЬ ПОЛЬЗОВАТЕЛЯ В БАЗЕ ДАННЫХ
                // (CreateAsync сам хэширует пароль!)
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Если получилось, СРАЗУ ЖЕ ВХОДИМ В СИСТЕМУ (создаем "куки")
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Возвращаем Ok(), чтобы JS (fetch) перезагрузил страницу
                    return Ok(new { message = "Регистрация успешна!" });
                }
                else
                {
                    // Если не получилось (например, "Email уже занят"),
                    // отправляем ошибки обратно в JS
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { message = string.Join("\n", errors) });
                }
            }

            // Если модель (ViewModel) невалидна (например, пароли не совпали)
            var modelErrors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new { message = string.Join("\n", modelErrors) });
        }


        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // ✅ ПЫТАЕМСЯ ВОЙТИ В СИСТЕМУ
                // (Он сам берет пароль, хэширует его и сравнивает с хэшем в базе)
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

                if (result.Succeeded)
                {
                    // Если пароль верный, возвращаем Ok()
                    return Ok(new { message = "Вход успешен!" });
                }
                else
                {
                    // Если пароль неверный
                    return BadRequest(new { message = "Неверный Email или пароль." });
                }
            }

            var modelErrors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new { message = string.Join("\n", modelErrors) });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // ✅ ВЫХОДИМ ИЗ СИСТЕМЫ (удаляем "куки")
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); // Перезагружаем главную
        }
    }
}