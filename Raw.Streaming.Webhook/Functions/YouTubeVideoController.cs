using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Model.Discord;
using Raw.Streaming.Webhook.Model.Youtube;
using Raw.Streaming.Webhook.Services;
using Raw.Streaming.Webhook.Translators;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System;

namespace Raw.Streaming.Webhook.Functions
{
    public class YouTubeVideoController
    {
        private const string WebhookEndpoint = "webhook/youtube/video-update";

        private readonly string _webhookTopic = AppSettings.YoutubeVideoTopic;
        private readonly string _channelId = AppSettings.YoutubeChannelId;
        private readonly string _discordChannelId = AppSettings.DiscordVideosChannelId;
        private readonly IYoutubeSubscriptionService _subscriptionService;

        public YouTubeVideoController(
            IYoutubeSubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [FunctionName("YoutubeVideoSubscribe")]
        public async Task YoutubeVideoSubscribe(
            [TimerTrigger("0 0 5 * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("StreamChangeSubscribe execution started");
                var callbackUrl = $"https://{AppSettings.WebSiteUrl}/api/{WebhookEndpoint}";
                await _subscriptionService.SubscribeAsync($"{_webhookTopic}{_channelId}", callbackUrl);
                logger.LogInformation("StreamChangeSubscribe execution succeeded");
            }
            catch (Exception e)
            {
                logger.LogError($"StreamChangeSubscribe execution failed: {e.Message}");
                throw;
            }
        }

        [FunctionName(nameof(YoutubeVideoWebhook))]
        [return: ServiceBus("%DiscordNotificationQueueName%", Connection = "StreamingServiceBus")]
        public ServiceBusMessage YoutubeVideoWebhook(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = WebhookEndpoint)] HttpRequest req,
            ILogger logger)
        {
            try
            {
                logger.LogInformation($"{nameof(YoutubeVideoWebhook)} execution started");
                var stream = req.Body;
                var data = ConvertAtomToSyndication(stream, logger);
                if (data.IsNewVideo(DateTimeOffset.UtcNow) && !string.IsNullOrWhiteSpace(data.Link))
                {
                    var notification = YoutubeFeedToDiscordNotificationTranslator.Translate(data);
                    var message = new DiscordMessage(_discordChannelId, notification);
                    return new ServiceBusMessage
                    {
                        Body = BinaryData.FromObjectAsJson(message),
                        MessageId = $"youtube-video-{data.VideoId}"
                    };
                }
                else
                {
                    logger.LogInformation($"{nameof(YoutubeVideoWebhook)} execution succeeded:{data.Title} is an old video so will not notify Discord\nPublished: {data.Published}\nUpdated: {data.Updated}");
                    return null;
                }
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(YoutubeVideoWebhook)} execution failed: {e.Message}");
                throw;
            }
        }

        private static YoutubeFeed ConvertAtomToSyndication(Stream stream, ILogger logger)
        {
            using var xmlReader = XmlReader.Create(stream);
            SyndicationFeed feed = SyndicationFeed.Load(xmlReader);
            logger.LogInformation($"Youtube feed content:\n{JsonConvert.SerializeObject(feed)}");
            var item = feed.Items.FirstOrDefault();
            return new YoutubeFeed()
            {
                ChannelId = GetElementExtensionValueByOuterName(item, "channelId"),
                VideoId = GetElementExtensionValueByOuterName(item, "videoId"),
                Title = item.Title.Text,
                Link = item.Links[0].Uri.ToString(),
                Published = item.PublishDate,
                Updated = item.LastUpdatedTime,
                Author = new Author
                {
                    Name = item.Authors[0].Name,
                    Uri = item.Authors[0].Uri
                }
            };
        }

        private static string GetElementExtensionValueByOuterName(SyndicationItem item, string outerName)
        {
            if (item.ElementExtensions.All(x => x.OuterName != outerName)) return null;
            return item.ElementExtensions.Single(x => x.OuterName == outerName).GetObject<XElement>().Value;
        }
    }
}
