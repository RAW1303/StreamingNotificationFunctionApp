using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Common.Model.Enums;
using Raw.Streaming.Webhook.Model.Twitch;
using Raw.Streaming.Webhook.Services;

namespace Raw.Streaming.Webhook.Functions
{
    [ServiceBusAccount("StreamingServiceBus")]
    public class TwitchHighlightsController
    {
        private readonly ITwitchApiService _twitchApiService;
        private readonly IMapper _mapper;

        public TwitchHighlightsController(
            ITwitchApiService twitchApiService,
            IMapper mapper)
        {
            _twitchApiService = twitchApiService;
            _mapper = mapper;
        }

        [FunctionName("NotifyTwitchHighlights")]
        [return: ServiceBus("%VideoQueueName%")]
        public async Task<ServiceBusMessage> NotifyTwitchHighlights(
            [TimerTrigger("%TwitchHighlightsTimerTrigger%")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("NotifyTwitchHighlights execution started");
                var startedAt = new DateTime(Math.Max(timer.ScheduleStatus.Last.Ticks, DateTime.UtcNow.AddHours(-25).Ticks));
                var startedAtUtc = DateTime.SpecifyKind(startedAt, DateTimeKind.Utc);
                var highlights = await GetHighlightsAsync(AppSettings.TwitchBroadcasterId, startedAtUtc, logger);
                var videos = _mapper.Map<IEnumerable<Video>>(highlights);
                var queueItem = new DiscordBotQueueItem<Video>(MessageType.Video, videos.ToArray());
                return new ServiceBusMessage
                {
                    Body = BinaryData.FromObjectAsJson(queueItem),
                    MessageId = $"twitch-highlights-{startedAtUtc}"
                };
            }
            catch (Exception e)
            {
                logger.LogError($"NotifyTwitchHighlights execution failed: {e.Message}");
                throw;
            }
        }

        private async Task<IEnumerable<TwitchVideo>> GetHighlightsAsync(string broadcasterId, DateTime startedAt, ILogger logger)
        {
            var videos = await _twitchApiService.GetHighlightsByBroadcasterAsync(broadcasterId);
            return videos.Where(video => video.PublishedAt >= startedAt && video.Viewable == "public").OrderBy(video => video.PublishedAt);
        }
    }
}
