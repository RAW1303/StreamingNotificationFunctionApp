using System;
using System.Diagnostics.CodeAnalysis;

namespace Raw.Streaming.Discord
{
    [ExcludeFromCodeCoverage]
    internal static class AppSettings
    {
        public static string DiscordApiUrl => Environment.GetEnvironmentVariable("DiscordApiUrl");
        public static string DiscordBotApplicationId => Environment.GetEnvironmentVariable("DiscordBotApplicationId");
        public static string DiscordBotToken => Environment.GetEnvironmentVariable("DiscordBotToken");
        public static string DiscordClipChannelId => Environment.GetEnvironmentVariable("DiscordClipChannelId");
        public static string DiscordEventsUrl => Environment.GetEnvironmentVariable("DiscordEventsUrl");
        public static string DiscordGuildId => Environment.GetEnvironmentVariable("DiscordGuildId");
        public static string DiscordNotificationGroupIds => Environment.GetEnvironmentVariable("DiscordNotificationGroupIds");
        public static string DiscordScheduleChannelId => Environment.GetEnvironmentVariable("DiscordScheduleChannelId");
        public static string DiscordStreamGoLiveChannelId => Environment.GetEnvironmentVariable("DiscordStreamGoLiveChannelId");
        public static string DiscordVideoChannelId => Environment.GetEnvironmentVariable("DiscordVideoChannelId");
    }
}
