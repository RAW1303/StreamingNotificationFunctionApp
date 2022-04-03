using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model.Twitch
{
    public class TwitchGame
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("box_art_url")]
        public string BoxArtUrl { get; set; }
    }
}
