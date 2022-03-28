using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model.Enums;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Model.Discord;
using Raw.Streaming.Webhook.Services;
using Raw.Streaming.Webhook.Translators;

namespace Raw.Streaming.Webhook.Functions
{
    public class TwitchClipController
    {
        private readonly ITwitchApiService _twitchApiService;

        public TwitchClipController(
            ITwitchApiService twitchApiService)
        {
            _twitchApiService = twitchApiService;
        }

        [FunctionName("NotifyTwitchClips")]
        [return: ServiceBus("%DiscordNotificationQueueName%", Connection = "StreamingServiceBus")]
        public async Task<IEnumerable<ServiceBusMessage>> NotifyTwitchClips(
            [TimerTrigger("%TwitchClipsTimerTrigger%")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("NotifyTwitchClips execution started");
                var startedAt = new DateTime(Math.Max(timer.ScheduleStatus.Last.Ticks, DateTime.UtcNow.AddMinutes(-10).Ticks));
                var startedAtUtc = DateTime.SpecifyKind(startedAt, DateTimeKind.Utc);
                var endedAtUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                var notifications = await GetClipNotificationsAsync(AppSettings.TwitchBroadcasterId, startedAtUtc, endedAtUtc, logger);
                return notifications.Select(n => {
                    var message = new DiscordMessage(MessageType.Clip, n);
                    return new ServiceBusMessage
                    {
                        Body = BinaryData.FromObjectAsJson(message),
                        MessageId = $"twitch-clips-{endedAtUtc:s}"
                    };
                });
            }
            catch (Exception e)
            {
                logger.LogError($"NotifyTwitchClips execution failed: {e.Message}");
                throw;
            }
        }

        private async Task<Notification[]> GetClipNotificationsAsync(string broadcasterId, DateTime startedAt, DateTime endedAt, ILogger logger)
        {
            var clips = await _twitchApiService.GetClipsByBroadcasterAsync(broadcasterId, startedAt, endedAt);
            return await Task.WhenAll(clips.OrderBy(x => x.CreatedAt).Select(async clip =>
            {
                var games = await _twitchApiService.GetGamesAsync(clip.GameId);
                return TwitchClipToDiscordNotificationTranslator.Translate(clip, games.First());
            }));
        }
    }
}
