using MimeKit;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using ElectronicsStore.BLL.Interfaces;

namespace ElectronicsStore.BLL.Realizations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Используем оператор ?? "", чтобы избежать null
            var senderName = _config["EmailSettings:SenderName"] ?? "ElectronicsHub";
            var senderEmail = _config["EmailSettings:SenderEmail"] ?? "";
            var mailServer = _config["EmailSettings:MailServer"] ?? "";
            var senderPassword = _config["EmailSettings:SenderPassword"] ?? "";

            var mailPort = _config.GetValue<int>("EmailSettings:MailPort");

            // Проверка настроек перед отправкой
            if (string.IsNullOrEmpty(senderEmail) || string.IsNullOrEmpty(mailServer))
            {
                // Если настройки пустые, просто выходим (чтобы сайт не падал при регистрации)
                // В реальном проекте тут нужен логгер
                return;
            }

            var emailMessage = new MimeMessage();
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
                catch
                {
                    // Игнорируем ошибки почты, чтобы не ломать регистрацию
                }
            }
        }
    }
}