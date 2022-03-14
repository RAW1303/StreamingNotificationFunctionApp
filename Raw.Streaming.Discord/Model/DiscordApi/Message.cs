using System.Text.Json.Serialization;

namespace Raw.Streaming.Discord.Model.DiscordApi
{
    public class Message : DiscordApiContent
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("embeds")]
        public Embed[] Embeds { get; set; }
    }
}
