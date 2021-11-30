namespace Raw.Streaming.Webhook.Model.Twitch.EventSub
{
    public class EventSubRequest<T> where T: Condition
    {
        public string Type { get; set; }
        public string Version { get; set; }
        public Transport Transport { get; set; }
        public T Condition { get; set; }

    }

    public class Transport
    {
        public string Method { get; set; }
        public string Callback { get; set; }
        public string Secret { get; set; }
    }

    public abstract class Condition
    {
    }

}
