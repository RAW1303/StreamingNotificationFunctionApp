using System.Diagnostics.CodeAnalysis;

namespace Raw.Streaming.Common.Model
{
    [ExcludeFromCodeCoverage]
    public class Clip : Entity
    {
        public string? Id { get; set; }
        public string? BroadcasterName { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string? CreatorName { get; set; }
        public string? EmbedUrl { get; set; }
        public string? GameName { get; set; }
        public string? Title { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Url { get; set; }
    }
}
