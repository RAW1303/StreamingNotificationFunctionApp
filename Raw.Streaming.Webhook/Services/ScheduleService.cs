using AutoMapper;
using Google.Apis.Logging;
using Raw.Streaming.Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services;
internal class ScheduleService : IScheduleService
{
    private readonly ITwitchApiService _twitchApiService;
    private readonly IYoutubeScheduleService _youtubeScheduleService;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public ScheduleService(ITwitchApiService twitchApiService, IYoutubeScheduleService youtubeScheduleService, IMapper mapper, ILogger logger)
    {
        _twitchApiService = twitchApiService;
        _youtubeScheduleService = youtubeScheduleService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<Event>> GetScheduleAsync(DateTimeOffset from, DateTimeOffset? to = null)
    {
        var tasks = new List<Task<IEnumerable<Event>>>() 
        {
            GetTwitchSchedule(from, to),
            GetYoutubeSchedule(from, to)
        };

        var result = await Task.WhenAll(tasks);
        return result.SelectMany(x => x);
    }

    private async Task<IEnumerable<Event>> GetTwitchSchedule(DateTimeOffset from, DateTimeOffset? to = null)
    {
        var twitchSchedule = await _twitchApiService.GetScheduleByBroadcasterIdAsync(AppSettings.TwitchBroadcasterId, from);
        var filteredSegments = twitchSchedule.SegmentsExcludingVaction.Where(seg => to is null || seg.StartTime <= to);
        return _mapper.Map<IEnumerable<Event>>(twitchSchedule);
    }

    private async Task<IEnumerable<Event>> GetYoutubeSchedule(DateTimeOffset from, DateTimeOffset? to = null)
    {
        var youtubeSchedule = await _youtubeScheduleService.GetUpcomingBroadcastsAsync();
        var filteredSchedule = youtubeSchedule.Where(x => x.LiveStreamingDetails.ScheduledStartTime >= from && (to is null || x.LiveStreamingDetails.ScheduledStartTime <= from));
        return _mapper.Map<IEnumerable<Event>>(youtubeSchedule);
    }
}
