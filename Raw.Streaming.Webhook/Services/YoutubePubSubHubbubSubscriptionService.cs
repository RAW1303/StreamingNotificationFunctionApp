using System;
using System.IO;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Raw.Streaming.Webhook.Exceptions;
using Raw.Streaming.Webhook.Model.Youtube;

namespace Raw.Streaming.Webhook.Services
{
    public class YoutubePubSubHubbubSubscriptionService : IYoutubeSubscriptionService
    {
        private readonly ILogger _logger;
        private readonly HttpClient _client;

        public YoutubePubSubHubbubSubscriptionService(ILogger<YoutubePubSubHubbubSubscriptionService> logger, HttpClient httpClient)
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

        public YoutubeFeed ProcessRequest(Stream content)
        {
            using var xmlReader = XmlReader.Create(content);
            var feed = SyndicationFeed.Load(xmlReader);
            _logger.LogInformation($"YouTube video feed content:\n{JsonConvert.SerializeObject(feed)}");
            return YoutubeFeed.Create(feed);
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
