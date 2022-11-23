using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model.Twitch.EventSub;

[ExcludeFromCodeCoverage]
public class BroadcastUserCondition: Condition
{
    [JsonPropertyName("broadcaster_user_id")]
    public string BroadcasterUserId { get; set; }
}
