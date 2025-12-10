using MimeKit;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using ElectronicsStore.BLL.Interfaces;
// Удален лишний using, который мог вызывать конфликты

namespace ElectronicsStore.BLL.Realizations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        // Имя метода изменено на SendEmailAsync
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            var senderName = _config["EmailSettings:SenderName"] ?? "ElectronicsHub";
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var mailServer = _config["EmailSettings:MailServer"];

            // Безопасное получение порта
            var mailPort = _config.GetValue<int>("EmailSettings:MailPort");
            var senderPassword = _config["EmailSettings:SenderPassword"];

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

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(mailServer, mailPort, false);
                    await client.AuthenticateAsync(senderEmail, senderPassword);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка при отправке письма: {ex.Message}");
                }
            }
        }
    }
}