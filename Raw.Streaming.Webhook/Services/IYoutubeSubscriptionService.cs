using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public interface IYoutubeSubscriptionService
    {
        Task SubscribeAsync(string topicUrl, string callbackUrl);
        Task UnsubscribeAsync(string topicUrl, string callbackUrl);
    }
}
