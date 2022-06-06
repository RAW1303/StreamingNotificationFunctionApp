using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Webhook.Services;

namespace Raw.Streaming.Webhook.Functions
{
    [ServiceBusAccount("StreamingServiceBus")]
    internal class TwitchClipController
    {
        private readonly ITwitchApiService _twitchApiService;
        private readonly IMapper _mapper;
        private readonly ILogger<TwitchClipController> _logger;

        public TwitchClipController(
            ITwitchApiService twitchApiService,
            IMapper mapper,
            ILogger<TwitchClipController> logger)
        {
            _twitchApiService = twitchApiService;
            _mapper = mapper;
            _logger = logger;
        }

        [ExcludeFromCodeCoverage]
        [FunctionName(nameof(NotifyTwitchClipsTrigger))]
        [return: ServiceBus("%ClipsQueueName%")]
        public async Task<ServiceBusMessage> NotifyTwitchClipsTrigger(
            [TimerTrigger("%TwitchClipsTimerTrigger%")] TimerInfo timer)
        {
            return await NotifyTwitchClips(timer.ScheduleStatus.Last, timer.ScheduleStatus.Next);
        }

        public async Task<ServiceBusMessage> NotifyTwitchClips(DateTimeOffset last, DateTimeOffset next)
        {
            try
            {
                _logger.LogInformation("NotifyTwitchClips execution started");
                var startedAt = last > next.AddMinutes(-10) ? last : next.AddMinutes(-10);
                var clips = await GetClipsAsync(AppSettings.TwitchBroadcasterId, startedAt, next);
                var queueItem = new DiscordBotQueueItem<Clip>(clips.ToArray());
                return new ServiceBusMessage
                {
                    Body = BinaryData.FromObjectAsJson(queueItem),
                    MessageId = $"twitch-clips-{last:s}"
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"NotifyTwitchClips execution failed: {e.Message}");
                throw;
            }
        }

        private async Task<IEnumerable<Clip>> GetClipsAsync(string broadcasterId, DateTimeOffset startedAt, DateTimeOffset endedAt)
        {
            var clips = await _twitchApiService.GetClipsByBroadcasterAsync(broadcasterId, startedAt, endedAt);
            var gameIds = clips.Select(x => x.GameId).Distinct();
            var games = await _twitchApiService.GetGamesAsync(gameIds.ToArray());
            return clips.Join(games, c => c.GameId, g => g.Id, (c, g) =>
            {
                var clip = _mapper.Map<Clip>(c);
                clip.GameName = g.Name;
                return clip;
            });
        }
    }
}
