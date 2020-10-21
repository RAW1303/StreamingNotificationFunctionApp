using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public interface ISubscriptionService
    {
        Task SubscribeAsync(string topic, string callbackUrl);
        Task UnsubscribeAsync(string topic, string callbackUrl);
    }
}
