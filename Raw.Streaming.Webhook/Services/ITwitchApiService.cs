using Raw.Streaming.Webhook.Model.Twitch;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public interface ITwitchApiService
    {
        Task<TwitchChannel> GetChannelInfoAsync(string broadcasterId);
        Task<IList<TwitchGame>> GetGamesAsync(params string[] gameIds);
        Task<IList<TwitchClip>> GetClipsByBroadcasterAsync(string broadcasterId, DateTime? startedAt = null, DateTime? endedAt = null);
        Task<IList<TwitchVideo>> GetHighlightsByBroadcasterAsync(string broadcasterId);
    }
}
