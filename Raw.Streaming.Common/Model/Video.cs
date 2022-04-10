using System.Text.Json.Serialization;

namespace Raw.Streaming.Common.Model
{
    public class Video : Entity
    {
        [JsonInclude]
        public string Id { get; set; }
        [JsonInclude]
        public string AuthorName { get; set; }
        [JsonInclude]
        public string Description { get; set; }
        [JsonInclude]
        public string Duration { get; set; }
        [JsonInclude]
        public string Title { get; set; }
        [JsonInclude]
        public string Url { get; set; }
    }
}
