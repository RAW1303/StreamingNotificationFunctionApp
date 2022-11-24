using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model.Twitch.EventSub;

[ExcludeFromCodeCoverage]
public class StreamOnlineEvent : Event
{
    public string Id { get; set; }

    [JsonPropertyName("broadcaster_user_id")]
    public string BroadcasterUserId { get; set; }

    [JsonPropertyName("broadcaster_user_login")]
    public string BroadcasterUserLogin { get; set; }

    [JsonPropertyName("broadcaster_user_name")]
    public string BroadcasterUserName { get; set; }
    public string Type { get; set; }

    [JsonPropertyName("started_at")]
    public string StartedAt { get; set; }
}
