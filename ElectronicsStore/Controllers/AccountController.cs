using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MailKit.Net.Smtp;
using MimeKit;

namespace ElectronicsStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMemoryCache _cache;

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
                var existingUserByName = await _userManager.FindByNameAsync(model.Login);
                if (existingUserByName != null)
                {
                    return BadRequest(new { message = "Такой логин уже занят." });
                }

                var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
                if (existingUserByEmail != null)
                {
                    return BadRequest(new { message = "Пользователь с такой почтой уже существует." });
                }

                var code = new Random().Next(1000, 9999).ToString();

                try
                {
                    await SendEmailAsync(model.Email, "Код подтверждения", $"Ваш код: {code}");
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = "Ошибка отправки письма: " + ex.Message });
                }

                _cache.Set(model.Email, new { Model = model, Code = code }, TimeSpan.FromMinutes(5));

                return Ok(new { needConfirm = true, email = model.Email });
            }
            return BadRequest(new { message = "Некорректные данные" });
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailViewModel data)
        {
            if (_cache.TryGetValue(data.Email, out object? cachedData) && cachedData != null) 
            {
                dynamic userData = cachedData;
                string correctCode = userData.Code;
                RegisterViewModel model = userData.Model;

                if (data.Code == correctCode)
                {
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
                        _cache.Remove(data.Email);
                        return Ok(new { message = "Регистрация успешна!" });
                    }
                    return BadRequest(new { message = "Ошибка создания: " + string.Join(", ", result.Errors.Select(e => e.Description)) });
                }
                return BadRequest(new { message = "Неверный код." });
            }

            return BadRequest(new { message = "Время действия кода истекло." });
        }

        private async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Electronics Store", "xixpsix2@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 465, true);
                await client.AuthenticateAsync("xixpsix2@gmail.com", "Bobokayustac20");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl }); 

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