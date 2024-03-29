﻿using Raw.Streaming.Webhook.Model.Twitch;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    internal interface ITwitchApiService
    {
        Task<TwitchChannel> GetChannelInfoAsync(string broadcasterId);
        Task<IList<TwitchGame>> GetGamesAsync(params string[] gameIds);
        Task<IList<TwitchClip>> GetClipsByBroadcasterAsync(string broadcasterId, DateTimeOffset? startedAt = null, DateTimeOffset? endedAt = null);
        Task<IList<TwitchVideo>> GetHighlightsByBroadcasterAsync(string broadcasterId);
        Task<TwitchSchedule> GetScheduleByBroadcasterIdAsync(string broadcasterId, DateTimeOffset? startTime = null);
    }
}
