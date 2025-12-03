using System.Threading.Tasks;

namespace CSS.Services
{
    public interface IPushService
    {
        Task SendToAllAsync(string title, string body);
        Task SendToUserAsync(string userId, string title, string body);
    }
}
