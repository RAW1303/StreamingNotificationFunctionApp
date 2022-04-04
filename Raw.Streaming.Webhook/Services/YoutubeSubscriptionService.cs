using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Exceptions;

namespace Raw.Streaming.Webhook.Services
{
    public class YoutubeSubscriptionService : IYoutubeSubscriptionService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _client;

        public YoutubeSubscriptionService(ILogger<YoutubeSubscriptionService> logger, HttpClient httpClient)
        {
            _logger = logger;
            _client = httpClient;
        }

        public async Task SubscribeAsync(string topicUrl, string callbackUrl)
        {
            await SendYoutubeWebookRequestAsync("subscribe", topicUrl, callbackUrl);
        }

        public async Task UnsubscribeAsync(string topicUrl, string callbackUrl)
        {
            await SendYoutubeWebookRequestAsync("unsubscribe", topicUrl, callbackUrl);
        }

        private async Task SendYoutubeWebookRequestAsync(string action, string topicUrl, string callbackUrl)
        {
            string postDataStr = $"hub.mode={action}" +
                $"&hub.verify_token={Guid.NewGuid()}" +
                $"&hub.verify=async" +
                $"&hub.callback={HttpUtility.UrlEncode(callbackUrl)}" +
                $"&hub.topic={HttpUtility.UrlEncode(topicUrl)}";

            var request = new HttpRequestMessage(HttpMethod.Post, AppSettings.YoutubeSubscriptionUrl)
            {
                Content = new StringContent(postDataStr, Encoding.UTF8, "application/x-www-form-urlencoded")
            };

            _logger.LogDebug($"Calling twitch subscription endpoint with content:\n{postDataStr}");
            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new TwitchApiException($"Error while subscribing:\n{postDataStr}\n{await response.Content.ReadAsStringAsync()}");
            }
        }
    }
}
