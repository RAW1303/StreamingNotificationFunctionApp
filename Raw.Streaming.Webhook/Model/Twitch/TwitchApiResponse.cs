using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Webhook.Model.Twitch
{
    [ExcludeFromCodeCoverage]
    public class TwitchApiResponse<T>
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }
    }
}
