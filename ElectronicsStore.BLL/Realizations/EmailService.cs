using MimeKit;
// Убираем лишние using, чтобы не путать компилятор
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using ElectronicsStore.BLL.Interfaces;
using ElectronicsStore.Domain.Response;

namespace ElectronicsStore.BLL.Realizations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmail(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            var senderName = _config["EmailSettings:SenderName"] ?? "ElectronicsHub";
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var mailServer = _config["EmailSettings:MailServer"];
            await Task.CompletedTask;

            // ИСПРАВЛЕНИЕ 1: Безопасное получение порта (убираем ошибку int.Parse)
            var mailPort = _config.GetValue<int>("EmailSettings:MailPort");

            var senderPassword = _config["EmailSettings:SenderPassword"];

            // Проверка, чтобы не упало с ошибкой, если настроек нет
            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(mailServer))
            {
                throw new Exception("Ошибка: Не заполнены настройки EmailSettings в appsettings.json");
            }

            emailMessage.From.Add(new MailboxAddress(senderName, senderEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;

            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            // ИСПРАВЛЕНИЕ 2: Явно указываем, что берем SmtpClient из MailKit
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    // Подключение (false - без SSL, true - с SSL, обычно для 587 порта false + StartTls, который MailKit делает сам)
                    await client.ConnectAsync(mailServer, mailPort, false);

                    // Аутентификация
                    await client.AuthenticateAsync(senderEmail, senderPassword);

                    // Отправка
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    // Логируем или пробрасываем ошибку, чтобы понять, что пошло не так при отправке
                    throw new Exception($"Ошибка при отправке письма: {ex.Message}");
                }
            }
        }
    }
}