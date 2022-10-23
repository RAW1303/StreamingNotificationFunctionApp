using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services;
internal interface IYoutubeScheduleService
{
    Task<IEnumerable<Video>> GetUpcomingBroadcastsAsync();
}
