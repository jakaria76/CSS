using System.Text.Json;
using WebPush;
using CSS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CSS.Services
{
    public class WebPushNotificationService : INotificationService
    {
        private readonly VapidSettings _vapid;
        private readonly IServiceScopeFactory _scopeFactory;

        public WebPushNotificationService(VapidSettings vapid, IServiceScopeFactory scopeFactory)
        {
            _vapid = vapid;
            _scopeFactory = scopeFactory;
        }

        public async Task SendToSubscriptionAsync(string endpoint, string p256dh, string auth, string payloadJson)
        {
            if (string.IsNullOrWhiteSpace(endpoint)) return;

            var subscription = new PushSubscription(endpoint, p256dh, auth);

            var vapidDetails = new VapidDetails(
                _vapid.Subject,
                _vapid.PublicKey,
                _vapid.PrivateKey
            );

            var webPushClient = new WebPushClient();

            try
            {
                await webPushClient.SendNotificationAsync(subscription, payloadJson, vapidDetails);
            }
            catch (WebPushException ex)
            {
                Console.WriteLine("WebPush error: " + ex.Message);
            }
        }

        public async Task SendToAllUsersAsync(string payloadJson)
        {
            // 🔥 Create NEW SCOPE (New DbContext instance)
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var users = await db.Users
                .Where(u => !string.IsNullOrEmpty(u.PushEndpoint))
                .Select(u => new { u.PushEndpoint, u.PushP256dh, u.PushAuth })
                .ToListAsync();

            foreach (var u in users)
            {
                await SendToSubscriptionAsync(u.PushEndpoint!, u.PushP256dh!, u.PushAuth!, payloadJson);
            }
        }
    }
}
