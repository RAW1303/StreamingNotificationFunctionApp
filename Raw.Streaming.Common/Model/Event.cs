using System.Diagnostics.CodeAnalysis;

namespace Raw.Streaming.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class Event : Entity
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string? Title { get; set; }
        public string? Game { get; set; }
        public string? Url { get; set; }
    }
}
