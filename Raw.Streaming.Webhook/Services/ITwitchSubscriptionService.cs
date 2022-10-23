
using Raw.Streaming.Webhook.Model.Twitch.EventSub;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public interface ITwitchSubscriptionService
    {
        Task SubscribeAsync<T>(string type, T condition, string callbackUrl) where T : Condition;
        EventSubRequest<T> GenerateSubscriptionRequest<T>(string type, T condition, string callbackUrl) where T : Condition;
    }
}
