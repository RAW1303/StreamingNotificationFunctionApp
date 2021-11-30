using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model.Discord
{
    public class Notification
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("embeds")]
        public Embed[] Embeds { get; set; }
    }
}
