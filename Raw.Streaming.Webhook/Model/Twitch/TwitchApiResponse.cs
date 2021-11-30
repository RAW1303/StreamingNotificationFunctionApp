using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model.Twitch
{
    public class TwitchApiResponse<T>
    {
        [JsonPropertyName("data")]
        public IList<T> Data { get; set; }
    }
}
