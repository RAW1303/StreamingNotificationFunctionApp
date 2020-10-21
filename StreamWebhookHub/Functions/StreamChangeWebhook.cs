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
using Raw.Streaming.Webhook.Model;
using Raw.Streaming.Webhook.Services;
using Raw.Streaming.Webhook.Translators;

namespace Raw.Streaming.Webhook.Functions
{
    public class TwitchStreamChangeWebhookController
    {
        private const string WebhookEndpoint = "webhook/twitch/stream-change";

        private readonly string _webhookTopic = AppSettings.TwitchStreamChangeTopic;
        private readonly string _discordwebhookId = AppSettings.DiscordStreamLiveWebhookId;
        private readonly string _discordwebhookToken = AppSettings.DiscordStreamLiveWebhookToken;
        private readonly IDiscordNotificationService _discordNotificationService;
        private readonly ITwitchApiService _twitchApiService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly TwitchStreamChangeToDiscordNotificationTranslator _translator;

        public TwitchStreamChangeWebhookController(
            ISubscriptionService subscriptionService, 
            IDiscordNotificationService discordNotificationService,
            ITwitchApiService twitchApiService,
            TwitchStreamChangeToDiscordNotificationTranslator translator)
        {
            _subscriptionService = subscriptionService;
            _discordNotificationService = discordNotificationService;
            _twitchApiService = twitchApiService;
            _translator = translator;
        }

        [FunctionName("StreamChangeSubscribe")]
        public async Task StreamChangeSubscribe(
            [TimerTrigger("0 0 5 * * 1", RunOnStartup = true)]TimerInfo myTimer,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("StreamChangeSubscribe execution started");
                var callbackUrl = $"https://{AppSettings.WebSiteUrl}/api/{WebhookEndpoint}";
                await _subscriptionService.SubscribeAsync(_webhookTopic, callbackUrl);
                logger.LogInformation("StreamChangeSubscribe execution succeeded");
            }
            catch(Exception e)
            {
                logger.LogError($"StreamChangeSubscribe execution failed: {e.Message}");
                throw;
            }
        }

        [FunctionName("StreamChangeWebhook")]
        public async Task<IActionResult> StreamChangeWebhook(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = WebhookEndpoint)] HttpRequest req,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("StreamChangeWebhook execution started");
                var requestContentString = await req.ReadAsStringAsync();
                logger.LogDebug($"StreamChangeWebhook request content:\n{requestContentString}");
                var requestContentObject = JsonSerializer.Deserialize<TwitchStreamChangeRequest>(requestContentString);
                if(requestContentObject.Data.Length < 1 || requestContentObject.Data[0].Type != "live")
                {
                    logger.LogInformation("StreamChangeSubscribe execution succeeded: Stream Stopped");
                    return new OkResult();
                }
                var twitchStreamChange = requestContentObject.Data.First();
                var games = await _twitchApiService.GetGames(twitchStreamChange.GameId);
                var notification = _translator.Translate(twitchStreamChange, games.First());
                logger.LogInformation("Sending stream change notification to discord server");
                await _discordNotificationService.SendNotification(_discordwebhookId, _discordwebhookToken, notification);
                logger.LogInformation("StreamChangeSubscribe execution succeeded: Notification sent");
                return new OkResult();
            }
            catch (Exception e)
            {
                logger.LogError($"StreamChangeSubscribe execution failed: {e.Message}");
                throw;
            }
        }
    }
}
