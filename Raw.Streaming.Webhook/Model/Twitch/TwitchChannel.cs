using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model.Twitch
{
    public class TwitchChannel
    {
        [JsonPropertyName("broadcaster_id")]
        public string BroadcasterId { get; set; }
        [JsonPropertyName("broadcaster_name")]
        public string BroadcasterName { get; set; }
        [JsonPropertyName("game_name")]
        public string GameName { get; set; }
        [JsonPropertyName("game_id")]
        public string GameId { get; set; }
        [JsonPropertyName("broadcaster_language")]
        public string BroadcasterLanguage { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("delay")]
        public int Delay { get; set; }
    }
}
