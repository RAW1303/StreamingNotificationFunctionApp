using System.Diagnostics.CodeAnalysis;

namespace Raw.Streaming.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class GoLive : Entity
    {
        public string? BroadcasterName { get; set; }
        public string? GameName { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
    }
}
