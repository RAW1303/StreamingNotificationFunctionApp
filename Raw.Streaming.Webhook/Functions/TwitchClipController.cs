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
    public class TwitchClipController
    {
        private readonly string _discordwebhookId = AppSettings.DiscordClipsWebhookId;
        private readonly string _discordwebhookToken = AppSettings.DiscordClipsWebhookToken;
        private readonly IDiscordNotificationService _discordNotificationService;
        private readonly ITwitchApiService _twitchApiService;
        private readonly TwitchClipToDiscordNotificationTranslator _translator;

        public TwitchClipController(
            IDiscordNotificationService discordNotificationService,
            ITwitchApiService twitchApiService,
            TwitchClipToDiscordNotificationTranslator translator)
        {
            _discordNotificationService = discordNotificationService;
            _twitchApiService = twitchApiService;
            _translator = translator;
        }


        [FunctionName("NotifyTwitchClipsHttp")]
        public async Task<IActionResult> NotifyTwitchClipsHttp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "twitch/notify-clips")] HttpRequest req,
            ILogger logger)
        {
            try
            {
                var broadcasterId = req.Query["broadcaster-id"];
                var startedAt = DateTime.SpecifyKind(Convert.ToDateTime(req.Query["started-at"]), DateTimeKind.Utc);
                var endedAt = DateTime.SpecifyKind(Convert.ToDateTime(req.Query["ended-at"]), DateTimeKind.Utc);
                logger.LogInformation("NotifyTwitchClips execution started");
                var notificationOut = await SendClipsAsync(broadcasterId, startedAt, endedAt, logger);
                return new OkObjectResult(notificationOut);
            }
            catch (Exception e)
            {
                logger.LogError($"NotifyTwitchClips execution failed: {e.Message}");
                throw;
            }
        }


        [FunctionName("NotifyTwitchClips")]
        public async Task NotifyTwitchClips(
            [TimerTrigger("0 */5 * * * *")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("NotifyTwitchClips execution started");
                var startedAt = new DateTime(Math.Max(timer.ScheduleStatus.Last.Ticks, DateTime.UtcNow.AddMinutes(-10).Ticks));
                var startedAtUtc = DateTime.SpecifyKind(startedAt, DateTimeKind.Utc);
                var endedAtUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                var notificationOut = await SendClipsAsync(AppSettings.TwitchBroadcasterId, startedAtUtc, endedAtUtc, logger);
            }
            catch (Exception e)
            {
                logger.LogError($"NotifyTwitchClips execution failed: {e.Message}");
                throw;
            }
        }

        private async Task<DiscordNotification> SendClipsAsync(string broadcasterId, DateTime startedAt, DateTime endedAt, ILogger logger)
        {
            var clips = await _twitchApiService.GetClipsByBroadcasterAsync(broadcasterId, startedAt, endedAt);
            var succeeded = 0;
            DiscordNotification notificationOut = null;
            await Task.WhenAll(clips.Select(async clip =>
            {
                var games = await _twitchApiService.GetGamesAsync(clip.GameId);
                var notification = _translator.Translate(clip, games.First());
                notificationOut = notification;
                try
                {
                    logger.LogInformation($"Clip notification {notification.Embeds[0].Title} started");
                    await _discordNotificationService.SendNotification(_discordwebhookId, _discordwebhookToken, notification);
                    succeeded++;
                    logger.LogInformation($"Clip notification {notification.Embeds[0].Title} succeeded");
                }
                catch (Exception e)
                {
                    logger.LogError($"Clip notification {notification.Embeds[0].Title} failed: {e.Message}");
                }
            }));
            return notificationOut;
        }
    }
}
