using System.Diagnostics.CodeAnalysis;

namespace Raw.Streaming.Webhook.Model.Twitch.EventSub;

[ExcludeFromCodeCoverage]
public class EventSubNotification<T> where T : Event
{
    public EventSubSubscription Subscription { get; set; }
    public T Event { get; set; }
}

[ExcludeFromCodeCoverage]
public abstract class Event
{
}
