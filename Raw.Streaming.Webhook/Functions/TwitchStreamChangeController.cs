using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model.Enums;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Model.Discord;
using Raw.Streaming.Webhook.Model.Twitch.EventSub;
using Raw.Streaming.Webhook.Services;
using Raw.Streaming.Webhook.Translators;

namespace Raw.Streaming.Webhook.Functions
{
    public class TwitchStreamChangeController : TwitchEventSubControllerBase<StreamOnlineEvent>
    {
        private const string WebhookEndpoint = "webhook/twitch/stream-change";

        private readonly string _webhookType = SubscriptionType.StreamOnline;
        private readonly string _broadcasterId = AppSettings.TwitchBroadcasterId;
        private readonly ITwitchApiService _twitchApiService;
        private readonly ITwitchSubscriptionService _subscriptionService;

        public TwitchStreamChangeController(
            ITwitchSubscriptionService subscriptionService, 
            ITwitchApiService twitchApiService,
            ILogger<TwitchStreamChangeController> logger): base(logger, SubscriptionType.StreamOnline)
        {
            _subscriptionService = subscriptionService;
            _twitchApiService = twitchApiService;
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
        [return: ServiceBus("%DiscordNotificationQueueName%", Connection = "StreamingServiceBus")]
        public async Task<ServiceBusMessage> StreamChangeWebhook(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = WebhookEndpoint)] HttpRequest req)
        {
            var from = DateTime.Now;
            var notification = await HandleRequestAsync(req);
            var message = new DiscordMessage(MessageType.StreamGoLive, notification);
            return new ServiceBusMessage
            {
                Body = BinaryData.FromObjectAsJson(message),
                MessageId = $"go-live-{from:yyyy-MM-ddTHH:mm:ss}"
            };
        }

        protected override async Task<Notification> HandleMessageAsync(StreamOnlineEvent message)
        {
            try
            {
                _logger.LogInformation("StreamChangeWebhook execution started");
                var channel = await _twitchApiService.GetChannelInfoAsync(message.BroadcasterUserId);
                var games = await _twitchApiService.GetGamesAsync(channel.GameId);
                return TwitchStreamChangeToDiscordNotificationTranslator.Translate(message, channel, games.First());
            }
            catch (Exception e)
            {
                _logger.LogError($"StreamChangeSubscribe execution failed: {e.Message}");
                throw;
            }
        }
    }
}
