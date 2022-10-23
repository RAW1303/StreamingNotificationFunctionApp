using System;
using System.Diagnostics.CodeAnalysis;

namespace Raw.Streaming.Webhook
{
    [ExcludeFromCodeCoverage]
    internal static class AppSettings
    {
        public static string TwitchApiChannelEndpoint => Environment.GetEnvironmentVariable("TwitchApiChannelEndpoint");
        public static string TwitchApiClipEndpoint => Environment.GetEnvironmentVariable("TwitchApiClipEndpoint");
        public static string TwitchApiGameEndpoint => Environment.GetEnvironmentVariable("TwitchApiGameEndpoint");
        public static string TwitchApiScheduleEndpoint => Environment.GetEnvironmentVariable("TwitchApiScheduleEndpoint");
        public static string TwitchApiUrl => Environment.GetEnvironmentVariable("TwitchApiUrl");
        public static string TwitchApiVideoEndpoint => Environment.GetEnvironmentVariable("TwitchApiVideoEndpoint");
        public static string TwitchBroadcasterId => Environment.GetEnvironmentVariable("TwitchBroadcasterId");
        public static string TwitchClientId => Environment.GetEnvironmentVariable("TwitchClientId");
        public static string TwitchClientSecret => Environment.GetEnvironmentVariable("TwitchClientSecret");
        public static string TwitchSubscriptionSecret => Environment.GetEnvironmentVariable("TwitchSubscriptionSecret");
        public static string TwitchSubscriptionUrl => Environment.GetEnvironmentVariable("TwitchSubscriptionUrl");
        public static string TwitchTokenUrl => Environment.GetEnvironmentVariable("TwitchTokenUrl");
        public static string WebSiteUrl => Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
        public static string YoutubeApiKey => Environment.GetEnvironmentVariable("YoutubeApiKey");
        public static string YoutubeChannelId => Environment.GetEnvironmentVariable("YoutubeChannelId");
        public static string YoutubeSubscriptionUrl => Environment.GetEnvironmentVariable("YoutubeSubscriptionUrl");
        public static string YoutubeVideoTopic => Environment.GetEnvironmentVariable("YoutubeVideoTopic");
    }
}
