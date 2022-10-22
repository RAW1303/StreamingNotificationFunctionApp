using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Google.Apis.YouTube.v3.SearchResource.ListRequest;

namespace Raw.Streaming.Webhook.Services;

internal class YoutubeScheduleService : IYoutubeScheduleService
{
    private YouTubeService _youtubeService;
    private readonly string _youtubeChannelId = AppSettings.YoutubeChannelId;

    private const string SEARCH_PARTS = "snippet";
    private const string VIDEO_PARTS = "snippet,liveStreamingDetails";


    public YoutubeScheduleService(YouTubeService youtubeService)
    {
        _youtubeService = youtubeService;
    }

    public async Task<IEnumerable<Video>> GetUpcomingBroadcastsAsync()
    {
        var broadcastIds = await GetUpcomingBroadcastIdsAsync();
        return await GetVideosByIdsAsync(broadcastIds);
    }

    private async Task<IEnumerable<string>> GetUpcomingBroadcastIdsAsync()
    {
        var broadcastSearch = _youtubeService.Search.List(SEARCH_PARTS);
        broadcastSearch.ChannelId = _youtubeChannelId;
        broadcastSearch.Type = "video";
        broadcastSearch.EventType = EventTypeEnum.Upcoming;
        var broadcasts = await broadcastSearch.ExecuteAsync();
        return broadcasts.Items.Select(x => x.Id.VideoId);
    }

    private async Task<IEnumerable<Video>> GetVideosByIdsAsync(IEnumerable<string> ids)
    {
        var videoQuery = _youtubeService.Videos.List(VIDEO_PARTS.Split(','));
        videoQuery.Id = ids.ToArray();
        var videos = await videoQuery.ExecuteAsync();
        return videos.Items;
    }
}
