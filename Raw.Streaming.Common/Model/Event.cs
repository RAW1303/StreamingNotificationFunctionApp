using System.Diagnostics.CodeAnalysis;

namespace Raw.Streaming.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class Event : Entity
    {
        public string? Id { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset? End { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public bool IsRecurring { get; set; }
    }
}
