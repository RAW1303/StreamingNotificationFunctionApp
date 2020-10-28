using Raw.Streaming.Webhook.Model;
using System;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public interface ITwitchApiService
    {
        Task<TwitchGame[]> GetGamesAsync(params string[] gameId);
        Task<TwitchClip[]> GetClipsByBroadcasterAsync(string broadcasterId, DateTime? startedAt = null, DateTime? endedAt = null);
        Task<TwitchVideo[]> GetHighlightsByBroadcasterAsync(string broadcasterId);
    }
}
