using Raw.Streaming.Webhook.Model.Youtube;
using System.IO;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public interface IYoutubeSubscriptionService
    {
        Task SubscribeAsync(string topicUrl, string callbackUrl);
        Task UnsubscribeAsync(string topicUrl, string callbackUrl);
        YoutubeFeed ProcessRequest(Stream content);
    }
}
