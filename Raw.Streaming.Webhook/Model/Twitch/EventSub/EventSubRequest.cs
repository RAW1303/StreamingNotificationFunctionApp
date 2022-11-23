using System.Diagnostics.CodeAnalysis;

namespace Raw.Streaming.Webhook.Model.Twitch.EventSub;

[ExcludeFromCodeCoverage]
public class EventSubRequest<T> where T: Condition
{
    public string Type { get; set; }
    public string Version { get; set; }
    public Transport Transport { get; set; }
    public T Condition { get; set; }

}

[ExcludeFromCodeCoverage]
public class Transport
{
    public string Method { get; set; }
    public string Callback { get; set; }
    public string Secret { get; set; }
}

[ExcludeFromCodeCoverage]
public abstract class Condition
{
}
