using System;
using System.Diagnostics.CodeAnalysis;

namespace Raw.Streaming.Webhook.Model;

[ExcludeFromCodeCoverage]
public class StreamEvent
{
    public DateTimeOffset Start { get; set; }
    public DateTimeOffset End { get; set; }
    public string Title { get; set; }
    public string Game { get; set; }
    public string Description { get; set; }
}
