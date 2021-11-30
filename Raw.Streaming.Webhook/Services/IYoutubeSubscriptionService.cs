using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public interface IYoutubeSubscriptionService : ISubscriptionService
    {
        Task SubscribeAsync(string topic, string callbackUrl);
        Task UnsubscribeAsync(string topic, string callbackUrl);
    }
}
