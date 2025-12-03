using System.Threading.Tasks;

namespace CSS.Services
{
    public interface INotificationService
    {
        Task SendToSubscriptionAsync(string endpoint, string p256dh, string auth, string payloadJson);
        Task SendToAllUsersAsync(string payloadJson);
    }
}
