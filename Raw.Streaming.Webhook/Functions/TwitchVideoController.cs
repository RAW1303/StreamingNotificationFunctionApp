using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Webhook.Model.Twitch;
using Raw.Streaming.Webhook.Services;

namespace Raw.Streaming.Webhook.Functions
{
    [ServiceBusAccount("StreamingServiceBus")]
    public class TwitchHighlightsController
    {
        private readonly ITwitchApiService _twitchApiService;
        private readonly IMapper _mapper;
        private readonly ILogger<TwitchHighlightsController> _logger;

        internal TwitchHighlightsController(
            ITwitchApiService twitchApiService,
            IMapper mapper,
            ILogger<TwitchHighlightsController> logger)
        {
            _twitchApiService = twitchApiService;
            _mapper = mapper;
            _logger = logger;
        }

        [FunctionName("NotifyTwitchHighlights")]
        [return: ServiceBus("%VideosQueueName%")]
        public async Task<ServiceBusMessage> NotifyTwitchHighlightsTrigger(
            [TimerTrigger("%TwitchHighlightsTimerTrigger%")] TimerInfo timer)
        {
                return await NotifyTwitchHighlights(timer.ScheduleStatus.Last, timer.ScheduleStatus.Next);
        }

        public async Task<ServiceBusMessage> NotifyTwitchHighlights(DateTimeOffset last, DateTimeOffset next)
        { 
            try
            {
                _logger.LogDebug("NotifyTwitchHighlights execution started");
                var startedAt = last > next.AddHours(-25) ? last : next.AddHours(-25);
                var highlights = await GetHighlightsAsync(AppSettings.TwitchBroadcasterId, startedAt);
                var videos = _mapper.Map<IEnumerable<Video>>(highlights);
                var queueItem = new DiscordBotQueueItem<Video>(videos.ToArray());
                return new ServiceBusMessage
                {
                    Body = BinaryData.FromObjectAsJson(queueItem),
                    MessageId = $"twitch-highlights-{startedAt}"
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"NotifyTwitchHighlights execution failed: {e.Message}");
                throw;
            }
        }

        private async Task<IEnumerable<TwitchVideo>> GetHighlightsAsync(string broadcasterId, DateTimeOffset startedAt)
        {
            var videos = await _twitchApiService.GetHighlightsByBroadcasterAsync(broadcasterId);
            return videos.Where(video => video.PublishedAt >= startedAt && video.Viewable == "public").OrderBy(video => video.PublishedAt);
        }
    }
}
