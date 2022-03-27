using System;

namespace Raw.Streaming.Discord
{
    internal static class AppSettings
    {
        public static string DiscordApiUrl { get; } = Environment.GetEnvironmentVariable("DiscordApiUrl");
        public static string DiscordBotToken { get; } = Environment.GetEnvironmentVariable("DiscordBotToken");
    }
}
