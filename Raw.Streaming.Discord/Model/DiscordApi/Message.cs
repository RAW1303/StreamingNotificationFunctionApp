using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Discord.Model.DiscordApi
{
    [ExcludeFromCodeCoverage]
    public class Message : DiscordApiContent
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("embeds")]
        public Embed[] Embeds { get; set; }
    }
}
