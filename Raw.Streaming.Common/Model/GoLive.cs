using System.Text.Json.Serialization;

namespace Raw.Streaming.Common.Model
{
    public class GoLive : Entity
    {
        [JsonInclude]
        public string BroadcasterName { get; set; }
        [JsonInclude]
        public string GameName { get; set; }
        [JsonInclude]
        public string Title { get; set; }
    }
}
