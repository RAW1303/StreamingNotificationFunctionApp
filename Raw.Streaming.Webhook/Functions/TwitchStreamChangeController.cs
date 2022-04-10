using System;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Common.Model.Enums;
using Raw.Streaming.Webhook.Model.Twitch.EventSub;
using Raw.Streaming.Webhook.Services;

namespace Raw.Streaming.Webhook.Functions
{
    [ServiceBusAccount("StreamingServiceBus")]
    public class TwitchStreamChangeController : TwitchEventSubControllerBase<StreamOnlineEvent, GoLive>
    {
        private const string WebhookEndpoint = "webhook/twitch/stream-change";

        private readonly string _webhookType = SubscriptionType.StreamOnline;
        private readonly string _broadcasterId = AppSettings.TwitchBroadcasterId;
        private readonly ITwitchApiService _twitchApiService;
        private readonly ITwitchSubscriptionService _subscriptionService;
        private readonly IMapper _mapper;

        public TwitchStreamChangeController(
            ITwitchSubscriptionService subscriptionService, 
            ITwitchApiService twitchApiService,
            ILogger<TwitchStreamChangeController> logger,
            IMapper mapper): base(logger, SubscriptionType.StreamOnline)
        {
            _subscriptionService = subscriptionService;
            _twitchApiService = twitchApiService;
            _mapper = mapper;
        }

        [FunctionName(nameof(StreamChangeSubscribe))]
        public async Task StreamChangeSubscribe(
            [TimerTrigger("0 0 5 * * 1", RunOnStartup = true)]TimerInfo myTimer,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("StreamChangeSubscribe execution started");
                var callbackUrl = $"https://{AppSettings.WebSiteUrl}/api/{WebhookEndpoint}";
                var condition = new BroadcastUserCondition
                {
                    BroadcasterUserId = _broadcasterId
                };

                await _subscriptionService.SubscribeAsync(_webhookType, condition, callbackUrl);
                logger.LogInformation("StreamChangeSubscribe execution succeeded");
            }
            catch(Exception e)
            {
                logger.LogError($"StreamChangeSubscribe execution failed: {e.Message}");
                throw;
            }
        }

        [FunctionName(nameof(StreamChangeWebhook))]
        [return: ServiceBus("%GoLiveQueueName%")]
        public async Task<ServiceBusMessage> StreamChangeWebhook(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = WebhookEndpoint)] HttpRequest req)
        {
            var from = DateTime.Now;
            var goLive = await HandleRequestAsync(req);
            var message = new DiscordBotQueueItem<GoLive>(MessageType.StreamGoLive, goLive);
            return new ServiceBusMessage
            {
                Body = BinaryData.FromObjectAsJson(message),
                MessageId = $"go-live-{from:yyyy-MM-ddTHH:mm:ss}"
            };
        }

        protected override async Task<GoLive> HandleMessageAsync(StreamOnlineEvent message)
        {
            try
            {
                _logger.LogInformation("StreamChangeWebhook execution started");
                var channel = await _twitchApiService.GetChannelInfoAsync(message.BroadcasterUserId);
                return _mapper.Map<GoLive>(channel);
            }
            catch (Exception e)
            {
                _logger.LogError($"StreamChangeSubscribe execution failed: {e.Message}");
                throw;
            }
        }
    }
}
