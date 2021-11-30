using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model.Twitch.EventSub
{
    public class BroadcastUserCondition: Condition
    {
        [JsonPropertyName("broadcaster_user_id")]
        public string BroadcasterUserId { get; set; }
    }
}
