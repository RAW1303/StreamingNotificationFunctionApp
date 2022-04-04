using System;

namespace Raw.Streaming.Webhook
{
    static class AppSettings
    {
        public static string GoogleCalendarApiKey { get; } = Environment.GetEnvironmentVariable("GoogleCalendarApiKey");
        public static string ScheduleGoogleCalendarId { get; } = Environment.GetEnvironmentVariable("ScheduleGoogleCalendarId");
        public static string TwitchApiChannelEndpoint { get; } = Environment.GetEnvironmentVariable("TwitchApiChannelEndpoint");
        public static string TwitchApiClipEndpoint { get; } = Environment.GetEnvironmentVariable("TwitchApiClipEndpoint");
        public static string TwitchApiGameEndpoint { get; } = Environment.GetEnvironmentVariable("TwitchApiGameEndpoint");
        public static string TwitchApiUrl { get; } = Environment.GetEnvironmentVariable("TwitchApiUrl");
        public static string TwitchApiVideoEndpoint { get; } = Environment.GetEnvironmentVariable("TwitchApiVideoEndpoint");
        public static string TwitchBroadcasterId { get; } = Environment.GetEnvironmentVariable("TwitchBroadcasterId");
        public static string TwitchClientId { get; } = Environment.GetEnvironmentVariable("TwitchClientId");
        public static string TwitchClientSecret { get; } = Environment.GetEnvironmentVariable("TwitchClientSecret");
        public static string TwitchSubscriptionSecret { get; } = Environment.GetEnvironmentVariable("TwitchSubscriptionSecret");
        public static string TwitchSubscriptionUrl { get; } = Environment.GetEnvironmentVariable("TwitchSubscriptionUrl");
        public static string TwitchTokenUrl { get; } = Environment.GetEnvironmentVariable("TwitchTokenUrl");
        public static string WebSiteUrl { get; } = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
        public static string YoutubeChannelId { get; } = Environment.GetEnvironmentVariable("YoutubeChannelId");
        public static string YoutubeSubscriptionUrl { get; } = Environment.GetEnvironmentVariable("YoutubeSubscriptionUrl");
        public static string YoutubeVideoTopic { get; } = Environment.GetEnvironmentVariable("YoutubeVideoTopic");
    }
}
