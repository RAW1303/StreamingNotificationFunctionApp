using System;

namespace Raw.Streaming.Webhook.Common
{
    static class AppSettings
    {
        public static string DiscordClipsWebhookId { get; } = Environment.GetEnvironmentVariable("DiscordClipsWebhookId");
        public static string DiscordClipsWebhookToken { get; } = Environment.GetEnvironmentVariable("DiscordClipsWebhookToken");
        public static string DiscordGameBoxSize { get; } = Environment.GetEnvironmentVariable("DiscordGameBoxSize");
        public static string DiscordHighlightsWebhookId { get; } = Environment.GetEnvironmentVariable("DiscordHighlightsWebhookId");
        public static string DiscordHighlightsWebhookToken { get; } = Environment.GetEnvironmentVariable("DiscordHighlightsWebhookToken");
        public static string DiscordNotificationGroupIds { get; } = Environment.GetEnvironmentVariable("DiscordNotificationGroupIds");
        public static string DiscordScheduleLiveWebhookId { get; } = Environment.GetEnvironmentVariable("DiscordScheduleLiveWebhookId");
        public static string DiscordScheduleLiveWebhookToken { get; } = Environment.GetEnvironmentVariable("DiscordScheduleLiveWebhookToken");
        public static string DiscordStreamLiveWebhookId { get; } = Environment.GetEnvironmentVariable("DiscordStreamLiveWebhookId");
        public static string DiscordStreamLiveWebhookToken { get; } = Environment.GetEnvironmentVariable("DiscordStreamLiveWebhookToken");
        public static string DiscordVideosWebhookId { get; } = Environment.GetEnvironmentVariable("DiscordVideosWebhookId");
        public static string DiscordVideosWebhookToken { get; } = Environment.GetEnvironmentVariable("DiscordVideosWebhookToken");
        public static string DiscordVideoThumbnailSize { get; } = Environment.GetEnvironmentVariable("DiscordVideoThumbnailSize");
        public static string DiscordWebhookUrl { get; } = Environment.GetEnvironmentVariable("DiscordWebhookUrl");
        public static string GoogleCalendarApiKey { get; } = Environment.GetEnvironmentVariable("GoogleCalendarApiKey");
        public static string ScheduleGoogleCalendarId { get; } = Environment.GetEnvironmentVariable("ScheduleGoogleCalendarId");
        public static string StreamAlertContent { get; } = Environment.GetEnvironmentVariable("StreamAlertContent");
        public static string TwitchApiChannelEndpoint { get; } = Environment.GetEnvironmentVariable("TwitchApiChannelEndpoint");
        public static string TwitchApiClipEndpoint { get; } = Environment.GetEnvironmentVariable("TwitchApiClipEndpoint");
        public static string TwitchApiGameEndpoint { get; } = Environment.GetEnvironmentVariable("TwitchApiGameEndpoint");
        public static string TwitchApiUrl { get; } = Environment.GetEnvironmentVariable("TwitchApiUrl");
        public static string TwitchApiVideoEndpoint { get; } = Environment.GetEnvironmentVariable("TwitchApiVideoEndpoint");
        public static string TwitchBroadcasterId { get; } = Environment.GetEnvironmentVariable("TwitchBroadcasterId");
        public static string TwitchClientId { get; } = Environment.GetEnvironmentVariable("TwitchClientId");
        public static string TwitchClientSecret { get; } = Environment.GetEnvironmentVariable("TwitchClientSecret");
        public static string TwitchStreamChangeTopic { get; } = Environment.GetEnvironmentVariable("TwitchStreamChangeTopic");
        public static string TwitchStreamOnlineType { get; } = Environment.GetEnvironmentVariable("TwitchStreamOnlineType");
        public static string TwitchSubscriptionSecret { get; } = Environment.GetEnvironmentVariable("TwitchSubscriptionSecret");
        public static string TwitchSubscriptionUrl { get; } = Environment.GetEnvironmentVariable("TwitchSubscriptionUrl");
        public static string TwitchTokenUrl { get; } = Environment.GetEnvironmentVariable("TwitchTokenUrl");
        public static string WebSiteUrl { get; } = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
        public static string YoutubeChannelId { get; } = Environment.GetEnvironmentVariable("YoutubeChannelId");
        public static string YoutubeSubscriptionUrl { get; } = Environment.GetEnvironmentVariable("YoutubeSubscriptionUrl");
        public static string YoutubeVideoTopic { get; } = Environment.GetEnvironmentVariable("YoutubeVideoTopic");
    }
}
