using System;
using System.Linq;
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
    public class TwitchHighlightsController
    {
        private readonly string _discordwebhookId = AppSettings.DiscordHighlightsWebhookId;
        private readonly string _discordwebhookToken = AppSettings.DiscordHighlightsWebhookToken;
        private readonly IDiscordNotificationService _discordNotificationService;
        private readonly ITwitchApiService _twitchApiService;
        private readonly TwitchVideoToDiscordNotificationTranslator _translator;

        public TwitchHighlightsController(
            IDiscordNotificationService discordNotificationService,
            ITwitchApiService twitchApiService,
            TwitchVideoToDiscordNotificationTranslator translator)
        {
            _discordNotificationService = discordNotificationService;
            _twitchApiService = twitchApiService;
            _translator = translator;
        }


        [FunctionName("NotifyTwitchHighlightsHttp")]
        public async Task<IActionResult> NotifyTwitchHighlightsHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "twitch/notify-highlights")] HttpRequest req,
            ILogger logger)
        {
            try
            {
                var broadcasterId = req.Query["broadcaster-id"];
                var startedAt = DateTime.SpecifyKind(Convert.ToDateTime(req.Query["started-at"]), DateTimeKind.Utc);
                var endedAt = DateTime.SpecifyKind(Convert.ToDateTime(req.Query["ended-at"]), DateTimeKind.Utc);
                logger.LogInformation("NotifyTwitchHighlightsHttp execution started");
                var notificationOut = await SendClipsAsync(broadcasterId, startedAt, endedAt, logger);
                return new OkObjectResult(notificationOut);
            }
            catch (Exception e)
            {
                logger.LogError($"NotifyTwitchHighlightsHttp execution failed: {e.Message}");
                throw;
            }
        }


        [FunctionName("NotifyTwitchHighlights")]
        public async Task<IActionResult> NotifyTwitchHighlights(
            [TimerTrigger("0 0 9 */1 * *")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("NotifyTwitchHighlights execution started");
                var startedAt = DateTime.SpecifyKind(timer.ScheduleStatus.Last, DateTimeKind.Utc);
                var endedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                var notificationOut = await SendClipsAsync(AppSettings.TwitchBroadcasterId, startedAt, endedAt, logger);
                return new OkObjectResult(notificationOut);
            }
            catch (Exception e)
            {
                logger.LogError($"NotifyTwitchHighlights execution failed: {e.Message}");
                throw;
            }
        }

        private async Task<DiscordNotification> SendClipsAsync(string broadcasterId, DateTime startedAt, DateTime endedAt, ILogger logger)
        {
            var videos = await _twitchApiService.GetHighlightsByBroadcasterAsync(broadcasterId, startedAt, endedAt);
            var succeeded = 0;
            DiscordNotification notificationOut = null;
            await Task.WhenAll(videos.Select(async video =>
            {
                var notification = _translator.Translate(video);
                notificationOut = notification;
                try
                {
                    logger.LogInformation($"Highlight notification {notification.Embeds[0].Title} started");
                    await _discordNotificationService.SendNotification(_discordwebhookId, _discordwebhookToken, notification);
                    succeeded++;
                    logger.LogInformation($"Highlight notification {notification.Embeds[0].Title} succeeded");
                }
                catch (Exception e)
                {
                    logger.LogError($"Highlight notification {notification.Embeds[0].Title} failed: {e.Message}");
                }
            }));
            return notificationOut;
        }
    }
}
