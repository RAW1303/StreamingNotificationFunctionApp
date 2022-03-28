using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model.Enums;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Model.Discord;
using Raw.Streaming.Webhook.Services;
using Raw.Streaming.Webhook.Translators;

namespace Raw.Streaming.Webhook.Functions
{
    public class TwitchHighlightsController
    {
        private readonly ITwitchApiService _twitchApiService;
        private readonly TwitchVideoToDiscordNotificationTranslator _translator;

        public TwitchHighlightsController(
            ITwitchApiService twitchApiService,
            TwitchVideoToDiscordNotificationTranslator translator)
        {
            _twitchApiService = twitchApiService;
            _translator = translator;
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
                var highlights = await GetHighlightsAsync(AppSettings.TwitchBroadcasterId, startedAtUtc, logger);
                var message = new DiscordMessage(MessageType.Video, highlights.ToArray());
            }
            catch (Exception e)
            {
                logger.LogError($"NotifyTwitchHighlights execution failed: {e.Message}");
                throw;
            }
        }

        private async Task<IEnumerable<Notification>> GetHighlightsAsync(string broadcasterId, DateTime startedAt, ILogger logger)
        {
            var videos = await _twitchApiService.GetHighlightsByBroadcasterAsync(broadcasterId);
            var filteredVideos = videos.Where(video => video.PublishedAt >= startedAt && video.Viewable == "public").OrderBy(video => video.PublishedAt);
            return filteredVideos.Select(video => _translator.Translate(video));
        }
    }
}
