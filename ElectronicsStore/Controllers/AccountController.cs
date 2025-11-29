using ElectronicsStore.Domain.Entity;
using ElectronicsStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options; // Обязательно добавьте этот using

namespace ElectronicsStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMemoryCache _cache;

        // Поле для хранения настроек почты
        private readonly EmailSettings _emailSettings;

        // Обновленный конструктор
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMemoryCache cache,
            IOptions<EmailSettings> emailSettings) // Получаем настройки
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cache = cache;
            _emailSettings = emailSettings.Value; // Сохраняем значения в поле
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            // ... (весь код метода Register оставляем без изменений) ...
            if (ModelState.IsValid)
            {
                // ... проверки на существование ...

                var code = new Random().Next(1000, 9999).ToString();

                try
                {
                    // Вызываем метод отправки (он теперь использует настройки)
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

        // ... метод ConfirmEmail оставляем без изменений ...

        // ВОТ ЗДЕСЬ ГЛАВНЫЕ ИЗМЕНЕНИЯ:
        private async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            // Берем имя и почту отправителя из настроек (_emailSettings)
            emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));

            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                // Берем сервер и порт из настроек
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, true);

                // Берем почту и пароль из настроек (никакого хардкода!)
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);

                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        // ... остальные методы (Login, Logout) оставляем без изменений ...
    }
}