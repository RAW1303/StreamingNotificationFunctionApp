using Raw.Streaming.Common.Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Raw.Streaming.Discord
{
    internal static class AppSettings
    {
        public static string DiscordApiUrl { get; } = Environment.GetEnvironmentVariable("DiscordApiUrl");
        public static string DiscordBotToken { get; } = Environment.GetEnvironmentVariable("DiscordBotToken");
        public static string DiscordStreamGoLiveChannelId { get; } = Environment.GetEnvironmentVariable("DiscordStreamGoLiveChannelId");
        public static string DiscordClipChannelId { get; } = Environment.GetEnvironmentVariable("DiscordClipChannelId");
        public static string DiscordVideoChannelId { get; } = Environment.GetEnvironmentVariable("DiscordVideoChannelId");
        public static string DiscordScheduleChannelId { get; } = Environment.GetEnvironmentVariable("DiscordScheduleChannelId");
    }
}
