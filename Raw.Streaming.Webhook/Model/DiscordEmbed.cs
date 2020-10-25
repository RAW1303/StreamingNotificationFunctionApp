using System;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model
{
    public class DiscordEmbed
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("color")]
        public int Color { get; set; }

        [JsonPropertyName("footer")]
        public DiscordEmbedFooter Footer { get; set; }

        [JsonPropertyName("image")]
        public DiscordEmbedImage Image { get; set; }

        [JsonPropertyName("thumbnail")]
        public DiscordEmbedThumbnail Thumbnail { get; set; }

        [JsonPropertyName("video")]
        public DiscordEmbedVideo Video { get; set; }

        [JsonPropertyName("author")]
        public DiscordEmbedAuthor Author { get; set; }

        [JsonPropertyName("fields")]
        public DiscordEmbedField[] Fields { get; set; }
    }

    public class DiscordEmbedField
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("inline")]
        public bool Inline { get; set; }
    }

    public class DiscordEmbedAuthor
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class DiscordEmbedImage
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class DiscordEmbedFooter
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class DiscordEmbedVideo
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }
    }

    public class DiscordEmbedThumbnail
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }
    }
}
