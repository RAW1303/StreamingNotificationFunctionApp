using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model.Twitch.EventSub
{
    public class EventSubSubscription
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        public string Status { get; set; }
        public int Cost { get; set; }
        public Condition Condition { get; set; }
        public Transport Transport { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
    }
}
