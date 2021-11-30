using System;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model.Discord
{
    public class Embed
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
        public DateTime? Timestamp { get; set; }

        [JsonPropertyName("color")]
        public int Color { get; set; }

        [JsonPropertyName("footer")]
        public EmbedFooter Footer { get; set; }

        [JsonPropertyName("image")]
        public EmbedImage Image { get; set; }

        [JsonPropertyName("thumbnail")]
        public EmbedThumbnail Thumbnail { get; set; }

        [JsonPropertyName("video")]
        public EmbedVideo Video { get; set; }

        [JsonPropertyName("author")]
        public EmbedAuthor Author { get; set; }

        [JsonPropertyName("fields")]
        public EmbedField[] Fields { get; set; }
    }

    public class EmbedField
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("inline")]
        public bool Inline { get; set; }
    }

    public class EmbedAuthor
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }

    public class EmbedImage
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class EmbedFooter
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public class EmbedVideo
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }
    }

    public class EmbedThumbnail
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }
    }
}
