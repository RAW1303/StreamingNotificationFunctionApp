using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model
{
    public class TwitchResponse<T>
    {
        [JsonPropertyName("data")]
        public T[] Data { get; set; }
    }
}
