using Raw.Streaming.Webhook.Model;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public interface IDiscordNotificationService
    {
        Task SendNotification(string webhookId, string webhookToken, DiscordNotification notification);
    }
}
