using System.Text.Json.Serialization;

namespace Raw.Streaming.Common.Model
{
    public class Event : Entity
    {
        [JsonInclude]
        public DateTime Start { get; set; }
        [JsonInclude]
        public DateTime End { get; set; }
        [JsonInclude]
        public string Title { get; set; }
        [JsonInclude]
        public string Game { get; set; }
        [JsonInclude]
        public string Url { get; set; }
    }
}
