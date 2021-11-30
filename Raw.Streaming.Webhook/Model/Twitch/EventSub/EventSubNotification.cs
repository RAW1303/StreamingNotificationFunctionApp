namespace Raw.Streaming.Webhook.Model.Twitch.EventSub
{
    public class EventSubNotification<T> where T : Event
    {
        public EventSubSubscription Subscription { get; set; }
        public T Event { get; set; }
    }

    public abstract class Event
    {
    }
}
