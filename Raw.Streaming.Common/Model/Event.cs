using System.Diagnostics.CodeAnalysis;

namespace Raw.Streaming.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class Event : Entity
    {
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public string? Title { get; set; }
        public string? Game { get; set; }
        public string? Url { get; set; }
    }
}
