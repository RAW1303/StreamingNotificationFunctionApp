using System;
using System.Diagnostics.CodeAnalysis;

namespace Raw.Streaming.Webhook.Model
{
    [ExcludeFromCodeCoverage]
    public class StreamEvent
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Title { get; set; }
        public string Game { get; set; }
        public string Description { get; set; }
    }
}
