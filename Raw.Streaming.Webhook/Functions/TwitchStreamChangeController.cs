using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Common;
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
        private readonly string _discordwebhookId = AppSettings.DiscordStreamLiveWebhookId;
        private readonly string _discordwebhookToken = AppSettings.DiscordStreamLiveWebhookToken;
        private readonly IDiscordNotificationService _discordNotificationService;
        private readonly ITwitchApiService _twitchApiService;
        private readonly ITwitchSubscriptionService _subscriptionService;

        public TwitchStreamChangeController(
            ITwitchSubscriptionService subscriptionService, 
            IDiscordNotificationService discordNotificationService,
            ITwitchApiService twitchApiService,
            ILogger<TwitchStreamChangeController> logger): base(logger, SubscriptionType.StreamOnline)
        {
            _subscriptionService = subscriptionService;
            _discordNotificationService = discordNotificationService;
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
        public async Task<IActionResult> StreamChangeWebhook(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = WebhookEndpoint)] HttpRequest req)
        {
            return await HandleRequestAsync(req);
        }

        protected override async Task<IActionResult> HandleMessageAsync(StreamOnlineEvent message)
        {
            try
            {
                _logger.LogInformation("StreamChangeWebhook execution started");
                var channel = await _twitchApiService.GetChannelInfoAsync(message.BroadcasterUserId);
                var games = await _twitchApiService.GetGamesAsync(channel.GameId);
                var notification = TwitchStreamChangeToDiscordNotificationTranslator.Translate(message, channel, games.First());
                _logger.LogInformation("Sending stream change notification to discord server");
                await _discordNotificationService.SendNotification(_discordwebhookId, _discordwebhookToken, notification);
                _logger.LogInformation("StreamChangeSubscribe execution succeeded: Notification sent");
                return new OkResult();
            }
            catch (Exception e)
            {
                _logger.LogError($"StreamChangeSubscribe execution failed: {e.Message}");
                throw;
            }
        }
    }
}
