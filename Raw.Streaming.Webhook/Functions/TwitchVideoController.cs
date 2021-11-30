using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Common;
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
                logger.LogInformation("NotifyTwitchHighlightsHttp execution started");
                await SendClipsAsync(broadcasterId, startedAt, logger);
                return new NoContentResult();
            }
            catch (Exception e)
            {
                logger.LogError($"NotifyTwitchHighlightsHttp execution failed: {e.Message}");
                throw;
            }
        }


        [FunctionName("NotifyTwitchHighlights")]
        public async Task NotifyTwitchHighlights(
            [TimerTrigger("%TwitchHighlightsTimerTrigger%")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("NotifyTwitchHighlights execution started");
                var startedAt = new DateTime(Math.Max(timer.ScheduleStatus.Last.Ticks, DateTime.UtcNow.AddHours(-25).Ticks));
                var startedAtUtc = DateTime.SpecifyKind(startedAt, DateTimeKind.Utc);
                await SendClipsAsync(AppSettings.TwitchBroadcasterId, startedAtUtc, logger);
            }
            catch (Exception e)
            {
                logger.LogError($"NotifyTwitchHighlights execution failed: {e.Message}");
                throw;
            }
        }

        private async Task SendClipsAsync(string broadcasterId, DateTime startedAt, ILogger logger)
        {
            var videos = await _twitchApiService.GetHighlightsByBroadcasterAsync(broadcasterId);
            var filteredVideos = videos.Where(video => video.PublishedAt >= startedAt && video.Viewable == "public").OrderBy(video => video.PublishedAt);
                
            foreach(var video in filteredVideos)
            {
                var notification = _translator.Translate(video);
                try
                {
                    logger.LogInformation($"Highlight notification {notification.Embeds[0].Title} started");
                    await _discordNotificationService.SendNotification(_discordwebhookId, _discordwebhookToken, notification);
                    logger.LogInformation($"Highlight notification {notification.Embeds[0].Title} succeeded");
                }
                catch (Exception e)
                {
                    logger.LogError($"Highlight notification {notification.Embeds[0].Title} failed: {e.Message}");
                }
            }
        }
    }
}
