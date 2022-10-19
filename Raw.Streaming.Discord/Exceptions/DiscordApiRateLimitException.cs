using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Raw.Streaming.Discord.Exceptions;

[ExcludeFromCodeCoverage]
[Serializable]
public sealed class DiscordApiRateLimitException : Exception
{
    private DiscordApiRateLimitException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public DiscordApiRateLimitException() : base() { }
    public DiscordApiRateLimitException(string message) : base(message) { }
}
