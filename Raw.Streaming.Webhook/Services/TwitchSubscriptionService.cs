using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Exceptions;
using Raw.Streaming.Webhook.Model;

namespace Raw.Streaming.Webhook.Services
{
    public class TwitchSubscriptionService : TwitchService, ISubscriptionService
    {
        private const int LeaseSeconds = 864000;

        public TwitchSubscriptionService(ILogger<TwitchSubscriptionService> logger, HttpClient httpClient): base(logger, httpClient)
        {
        }

        public async Task SubscribeAsync(string topic, string callbackUrl)
        {
            await SendTwitchWebookRequestAsync("subscribe", topic, callbackUrl);
        }

        public async Task UnsubscribeAsync(string topic, string callbackUrl)
        {
            await SendTwitchWebookRequestAsync("unsubscribe", topic, callbackUrl);
        }

        private async Task SendTwitchWebookRequestAsync(string action, string topic, string callbackUrl)
        {
            var subscriptionRequest = new HubSubscriptionRequest
            {
                Callback = callbackUrl,
                Mode = action,
                Topic = topic,
                LeaseSeconds = LeaseSeconds
            };

            var subscriptionRequestJson = JsonSerializer.Serialize(subscriptionRequest);
            var request = new HttpRequestMessage(HttpMethod.Post, AppSettings.TwitchSubscriptionUrl)
            {
                Content = new StringContent(subscriptionRequestJson, Encoding.UTF8, "application/json")
            };

            var scope = "user:read:broadcast";
            request.Headers.Add("Authorization", $"Bearer {await GetTwitchToken(scope)}");
            request.Headers.Add("client-id", AppSettings.TwitchClientId);

            _logger.LogInformation($"Calling twitch subscription endpoint with content:\n{subscriptionRequestJson}");
            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new TwitchApiException($"Error while subscribing: {await response.Content.ReadAsStringAsync()}");
            }
        }
    }
}
