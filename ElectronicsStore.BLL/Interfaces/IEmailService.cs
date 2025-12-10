using ElectronicsStore.Domain.Response;
using System.Threading.Tasks;

namespace ElectronicsStore.BLL.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string email, string subject, string message);
    }
}