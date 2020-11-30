using System;

namespace Raw.Streaming.Webhook.Common
{
    static class AppSettings
    {
        public static string WebSiteUrl { get; } = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
        public static string TwitchApiUrl { get; } = Environment.GetEnvironmentVariable("TwitchApiUrl");
        public static string TwitchClientId { get; } = Environment.GetEnvironmentVariable("TwitchClientId");
        public static string TwitchClientSecret { get; } = Environment.GetEnvironmentVariable("TwitchClientSecret");
        public static string TwitchSubscriptionUrl { get; } = Environment.GetEnvironmentVariable("TwitchSubscriptionUrl");
        public static string TwitchBroadcasterId { get; } = Environment.GetEnvironmentVariable("TwitchBroadcasterId");
        public static string TwitchApiGameEndpoint { get; } = Environment.GetEnvironmentVariable("TwitchApiGameEndpoint");
        public static string TwitchApiClipEndpoint { get; } = Environment.GetEnvironmentVariable("TwitchApiClipEndpoint");
        public static string TwitchApiVideoEndpoint { get; } = Environment.GetEnvironmentVariable("TwitchApiVideoEndpoint");
        public static string TwitchStreamChangeTopic { get; } = Environment.GetEnvironmentVariable("TwitchStreamChangeTopic");
        public static string TwitchTokenUrl { get; } = Environment.GetEnvironmentVariable("TwitchTokenUrl");
        public static string DiscordWebhookUrl { get; } = Environment.GetEnvironmentVariable("DiscordWebhookUrl");
        public static string DiscordClipsWebhookId { get; } = Environment.GetEnvironmentVariable("DiscordClipsWebhookId");
        public static string DiscordClipsWebhookToken { get; } = Environment.GetEnvironmentVariable("DiscordClipsWebhookToken");
        public static string DiscordHighlightsWebhookId { get; } = Environment.GetEnvironmentVariable("DiscordHighlightsWebhookId");
        public static string DiscordHighlightsWebhookToken { get; } = Environment.GetEnvironmentVariable("DiscordHighlightsWebhookToken");
        public static string DiscordStreamLiveWebhookId { get; } = Environment.GetEnvironmentVariable("DiscordStreamLiveWebhookId");
        public static string DiscordStreamLiveWebhookToken { get; } = Environment.GetEnvironmentVariable("DiscordStreamLiveWebhookToken");
        public static string GameBoxSize { get; } = Environment.GetEnvironmentVariable("GameBoxSize");
        public static string VideoThumbnailSize { get; } = Environment.GetEnvironmentVariable("VideoThumbnailSize");
    }
}
