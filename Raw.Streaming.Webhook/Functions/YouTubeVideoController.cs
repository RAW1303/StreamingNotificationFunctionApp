using AutoMapper;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Services;
using System;
using System.Threading.Tasks;
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
        private readonly ILogger<YouTubeVideoController> _logger;

        public YouTubeVideoController(
            IYoutubeSubscriptionService subscriptionService,
            IMapper mapper,
            ILogger<YouTubeVideoController> logger)
        {
            _subscriptionService = subscriptionService;
            _mapper = mapper;
            _logger = logger;
        }

        [FunctionName(nameof(YoutubeVideoSubscribeTrigger))]
        public async Task YoutubeVideoSubscribeTrigger(
            [TimerTrigger("0 0 5 * * *", RunOnStartup = true)] TimerInfo myTimer)
        {
            await YoutubeVideoSubscribe();
        }

        [FunctionName(nameof(YoutubeVideoWebhook))]
        [return: ServiceBus("%VideosQueueName%")]
        public ServiceBusMessage YoutubeVideoWebhook(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = WebhookEndpoint)] HttpRequest req)
        {
            try
            {
                _logger.LogDebug($"{nameof(YoutubeVideoWebhook)} execution started");
                var data = _subscriptionService.ProcessRequest(req.Body);

                if (data.IsNewVideo() && !string.IsNullOrWhiteSpace(data.Link))
                {
                    var video = _mapper.Map<Video>(data);
                    var queueItem = new DiscordBotQueueItem<Video>(video);
                    _logger.LogInformation($"{nameof(YoutubeVideoWebhook)} execution succeeded:{data.Title} has been sent to Discord");
                    return new ServiceBusMessage
                    {
                        Body = BinaryData.FromObjectAsJson(queueItem),
                        MessageId = $"youtube-video-{video.Id}"
                    };
                }
                else
                {
                    _logger.LogInformation($"{nameof(YoutubeVideoWebhook)} execution succeeded:{data.Title} is an old video so will not notify Discord\nPublished: {data.Published}\nUpdated: {data.Updated}");
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(YoutubeVideoWebhook)} execution failed: {e.Message}");
                throw;
            }
        }

        public async Task YoutubeVideoSubscribe()
        {
            try
            {
                _logger.LogDebug("StreamChangeSubscribe execution started");
                var callbackUrl = $"https://{AppSettings.WebSiteUrl}/api/{WebhookEndpoint}";
                await _subscriptionService.SubscribeAsync($"{_webhookTopic}{_channelId}", callbackUrl);
                _logger.LogDebug("StreamChangeSubscribe execution succeeded");
            }
            catch (Exception e)
            {
                _logger.LogError($"StreamChangeSubscribe execution failed: {e.Message}");
                throw;
            }
        }
    }
}
