using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory; // Нужен для хранения кода
using MailKit.Net.Smtp; // Нужен для отправки почты
using MimeKit;

namespace ElectronicsStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMemoryCache _cache; // Временная память

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMemoryCache cache)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Проверка: занят ли Логин?
                var existingUserByName = await _userManager.FindByNameAsync(model.Login);
                if (existingUserByName != null)
                {
                    return BadRequest(new { message = "Такой логин уже занят." });
                }

                // 2. Проверка: занят ли Email? (Требование методички)
                var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingUserByEmail != null)
                {
                    return BadRequest(new { message = "Пользователь с такой почтой уже существует." });
                }

                // 3. Генерация кода (4-6 цифр)
                var code = new Random().Next(1000, 9999).ToString();

                // 4. Отправка письма (как в методичке, глава 18)
                try
                {
                    await SendEmailAsync(model.Email, "Код подтверждения", $"Ваш код: {code}");
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Ошибка отправки письма: " + ex.Message });
                }

                // 5. Сохраняем данные во временную память на 5 минут, чтобы проверить код позже
                // Ключ - это почта, Значение - объект с данными и кодом
                _cache.Set(model.Email, new { Model = model, Code = code }, TimeSpan.FromMinutes(5));

                // Возвращаем ОК, но не создаем пользователя в БД!
                // Фронтенд должен теперь показать форму ввода кода.
                return Ok(new { needConfirm = true, email = model.Email });
            }
            return BadRequest(new { message = "Некорректные данные" });
        }

        // Новый метод для проверки кода
        [HttpPost]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailViewModel data)
        {
            // Пытаемся найти данные во временной памяти по почте
            if (_cache.TryGetValue(data.Email, out object cachedData))
            {
                // Используем dynamic, чтобы достать свойства анонимного объекта
                dynamic userData = cachedData;
                string correctCode = userData.Code;
                RegisterViewModel model = userData.Model;

                if (data.Code == correctCode)
                {
                    // КОД ВЕРНЫЙ! Теперь создаем пользователя в БД
                    var user = new ApplicationUser
                    {
                        Email = model.Email,
                        UserName = model.Login,
                        EmailConfirmed = true,
                        FirstName = "",
                        LastName = ""
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _cache.Remove(data.Email); // Чистим память
                        return Ok(new { message = "Регистрация успешна!" });
                    }
                    return BadRequest(new { message = "Ошибка создания пользователя в БД." });
                }
                return BadRequest(new { message = "Неверный код." });
            }

            return BadRequest(new { message = "Время действия кода истекло. Попробуйте снова." });
        }

        // Метод отправки почты (MailKit)
        private async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            // ВАЖНО: Укажите здесь вашу почту и имя
            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "ВАША_ПОЧТА@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                // Настройки для Gmail (нужен пароль приложения, см. методичку рис. 168)
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("ВАША_ПОЧТА@gmail.com", "ВАШ_ПАРОЛЬ_ПРИЛОЖЕНИЯ");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        // ... (Методы Login и Logout оставьте без изменений или добавьте проверку почты и там)
        [HttpGet]
        public IActionResult Login(string returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.LoginOrEmail);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(model.LoginOrEmail);
                }

                if (user == null)
                {
                    return BadRequest(new { message = "Пользователь не найден." });
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return Ok(new { email = user.Email, message = "Вход успешен!" });
                }
                return BadRequest(new { message = "Неверный пароль." });
            }
            return BadRequest(new { message = "Введите логин и пароль." });
        }

        [HttpPost]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}