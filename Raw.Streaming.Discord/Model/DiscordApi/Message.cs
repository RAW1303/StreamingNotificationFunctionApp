using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Discord.Model.DiscordApi
{
    [ExcludeFromCodeCoverage]
    internal class Message : DiscordApiContent
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("embeds")]
        public Embed[]? Embeds { get; set; }

        [JsonPropertyName("author")]
        public User? Author { get; set; }
    }
}
