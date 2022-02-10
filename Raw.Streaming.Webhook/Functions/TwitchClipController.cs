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
    public class TwitchClipController
    {
        private readonly string _discordwebhookId = AppSettings.DiscordClipsWebhookId;
        private readonly string _discordwebhookToken = AppSettings.DiscordClipsWebhookToken;
        private readonly IDiscordNotificationService _discordNotificationService;
        private readonly ITwitchApiService _twitchApiService;

        public TwitchClipController(
            IDiscordNotificationService discordNotificationService,
            ITwitchApiService twitchApiService)
        {
            _discordNotificationService = discordNotificationService;
            _twitchApiService = twitchApiService;
        }

        [FunctionName("NotifyTwitchClips")]
        public async Task NotifyTwitchClips(
            [TimerTrigger("%TwitchClipsTimerTrigger%")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("NotifyTwitchClips execution started");
                var startedAt = new DateTime(Math.Max(timer.ScheduleStatus.Last.Ticks, DateTime.UtcNow.AddMinutes(-10).Ticks));
                var startedAtUtc = DateTime.SpecifyKind(startedAt, DateTimeKind.Utc);
                var endedAtUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                await SendClipsAsync(AppSettings.TwitchBroadcasterId, startedAtUtc, endedAtUtc, logger);
            }
            catch (Exception e)
            {
                logger.LogError($"NotifyTwitchClips execution failed: {e.Message}");
                throw;
            }
        }

        private async Task SendClipsAsync(string broadcasterId, DateTime startedAt, DateTime endedAt, ILogger logger)
        {
            var clips = await _twitchApiService.GetClipsByBroadcasterAsync(broadcasterId, startedAt, endedAt);
            var succeeded = 0;
            await Task.WhenAll(clips.OrderBy(x => x.CreatedAt).Select(async clip =>
            {
                var games = await _twitchApiService.GetGamesAsync(clip.GameId);
                var notification = TwitchClipToDiscordNotificationTranslator.Translate(clip, games.First());
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
        }
    }
}
