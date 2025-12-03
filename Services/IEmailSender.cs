using System.Threading.Tasks;

namespace CSS.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
        Task SendToAllUsers(string subject, string htmlMessage);
    }
}
