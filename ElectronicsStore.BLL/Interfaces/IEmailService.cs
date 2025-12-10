using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Interfaces
{
    public interface IEmailService
    {
        // Метод называется SendEmailAsync
        Task SendEmailAsync(string email, string subject, string message);
    }
}