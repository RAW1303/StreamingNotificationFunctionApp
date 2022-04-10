using System.Text.Json.Serialization;

namespace Raw.Streaming.Common.Model
{
    public class Clip : Entity
    {
        [JsonInclude]
        public string Id { get; set; }
        [JsonInclude]
        public string BroadcasterName { get; set; }
        [JsonInclude]
        public DateTime CreatedAt { get; set; }
        [JsonInclude]
        public string CreatorName { get; set; }
        [JsonInclude]
        public string EmbedUrl { get; set; }
        [JsonInclude]
        public string GameName { get; set; }
        [JsonInclude]
        public string Title { get; set; }
        [JsonInclude]
        public string ThumbnailUrl { get; set; }
        [JsonInclude]
        public string Url { get; set; }
    }
}
