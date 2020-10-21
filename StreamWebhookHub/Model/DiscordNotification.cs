using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model
{
    public class DiscordNotification
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("embeds")]
        public DiscordEmbed[] Embeds { get; set; }
    }
}
