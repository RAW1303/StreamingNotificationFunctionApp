using AutoMapper;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Raw.Streaming.Webhook.Model.Youtube;
using Raw.Streaming.Webhook.Services;
using System;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using Raw.Streaming.Common.Model;

namespace Raw.Streaming.Webhook.Functions
{
    [ServiceBusAccount("StreamingServiceBus")]
    public class YouTubeVideoController
    {
        private const string WebhookEndpoint = "webhook/youtube/video-update";

        private readonly string _webhookTopic = AppSettings.YoutubeVideoTopic;
        private readonly string _channelId = AppSettings.YoutubeChannelId;
        private readonly IYoutubeSubscriptionService _subscriptionService;
        private readonly IMapper _mapper;

        public YouTubeVideoController(
            IYoutubeSubscriptionService subscriptionService,
            IMapper mapper)
        {
            _subscriptionService = subscriptionService;
            _mapper = mapper;
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
        [return: ServiceBus("%VideosQueueName%")]
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
                    var video = _mapper.Map<Video>(data);
                    var queueItem = new DiscordBotQueueItem<Video>(video);
                    return new ServiceBusMessage
                    {
                        Body = BinaryData.FromObjectAsJson(queueItem),
                        MessageId = $"youtube-video-{video.Id}"
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
            var item = feed.Items.First();
            return new YoutubeFeed()
            {
                ChannelId = GetElementExtensionValueByOuterName(item, "channelId"),
                VideoId = GetElementExtensionValueByOuterName(item, "videoId"),
                Title = item.Title?.Text,
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
