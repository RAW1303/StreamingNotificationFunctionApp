using System;
using System.Collections.Generic;
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
    public class TwitchClipController
    {
        private readonly ITwitchApiService _twitchApiService;
        private readonly IMapper _mapper;

        public TwitchClipController(
            ITwitchApiService twitchApiService,
            IMapper mapper)
        {
            _twitchApiService = twitchApiService;
            _mapper = mapper;
        }

        [FunctionName("NotifyTwitchClips")]
        [return: ServiceBus("%ClipsQueueName%")]
        public async Task<ServiceBusMessage> NotifyTwitchClips(
            [TimerTrigger("%TwitchClipsTimerTrigger%")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("NotifyTwitchClips execution started");
                var startedAt = new DateTime(Math.Max(timer.ScheduleStatus.Last.Ticks, DateTime.UtcNow.AddMinutes(-10).Ticks));
                var startedAtUtc = DateTime.SpecifyKind(startedAt, DateTimeKind.Utc);
                var endedAtUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
                var clips = await GetClipsAsync(AppSettings.TwitchBroadcasterId, startedAtUtc, endedAtUtc);
                var queueItem = new DiscordBotQueueItem<Clip>(clips.ToArray());
                return new ServiceBusMessage
                {
                    Body = BinaryData.FromObjectAsJson(queueItem),
                    MessageId = $"twitch-clips-{endedAtUtc:s}"
                };
            }
            catch (Exception e)
            {
                logger.LogError($"NotifyTwitchClips execution failed: {e.Message}");
                throw;
            }
        }

        private async Task<IEnumerable<Clip>> GetClipsAsync(string broadcasterId, DateTime startedAt, DateTime endedAt)
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
