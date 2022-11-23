using System.Diagnostics.CodeAnalysis;

namespace Raw.Streaming.Webhook.Model.Twitch.EventSub;

[ExcludeFromCodeCoverage]
public static class SubscriptionType
{
    public const string StreamOnline = "stream.online";
    public const string StreamOffline = "stream.offline";
}
