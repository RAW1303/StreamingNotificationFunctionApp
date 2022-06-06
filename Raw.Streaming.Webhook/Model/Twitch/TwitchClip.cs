using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model.Twitch
{
    [ExcludeFromCodeCoverage]
    public class TwitchClip
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("broadcaster_id")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("broadcaster_name")]
        public string BroadcasterName { get; set; }
        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
        [JsonPropertyName("creator_id")]
        public string CreatorId { get; set; }
        [JsonPropertyName("creator_name")]
        public string CreatorName { get; set; }
        [JsonPropertyName("embed_url")]
        public string EmbedUrl { get; set; }
        [JsonPropertyName("game_id")]
        public string GameId { get; set; }
        [JsonPropertyName("language")]
        public string Language { get; set; }
        [JsonPropertyName("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("url")]
        public string Url { get; set; }
        [JsonPropertyName("video_id")]
        public string VideoId { get; set; }
        [JsonPropertyName("view_count")]
        public int ViewCount { get; set; }
    }
}
