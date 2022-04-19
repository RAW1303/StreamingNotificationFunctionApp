using System;
using System.Diagnostics.CodeAnalysis;

namespace Raw.Streaming.Discord
{
    [ExcludeFromCodeCoverage]
    internal static class AppSettings
    {
        public static string DiscordApiUrl { get; } = Environment.GetEnvironmentVariable("DiscordApiUrl");
        public static string DiscordBotToken { get; } = Environment.GetEnvironmentVariable("DiscordBotToken");
        public static string DiscordClipChannelId { get; } = Environment.GetEnvironmentVariable("DiscordClipChannelId");
        public static string DiscordNotificationGroupIds { get; } = Environment.GetEnvironmentVariable("DiscordNotificationGroupIds");
        public static string DiscordScheduleChannelId { get; } = Environment.GetEnvironmentVariable("DiscordScheduleChannelId");
        public static string DiscordStreamGoLiveChannelId { get; } = Environment.GetEnvironmentVariable("DiscordStreamGoLiveChannelId");
        public static string DiscordVideoChannelId { get; } = Environment.GetEnvironmentVariable("DiscordVideoChannelId");
    }
}
