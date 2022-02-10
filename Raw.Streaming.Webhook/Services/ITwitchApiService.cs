using Raw.Streaming.Webhook.Model.Twitch;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public interface ITwitchApiService
    {
        Task<Channel> GetChannelInfoAsync(string broadcasterId);
        Task<IList<Game>> GetGamesAsync(params string[] gameIds);
        Task<IList<Clip>> GetClipsByBroadcasterAsync(string broadcasterId, DateTime? startedAt = null, DateTime? endedAt = null);
        Task<IList<Video>> GetHighlightsByBroadcasterAsync(string broadcasterId);
    }
}
