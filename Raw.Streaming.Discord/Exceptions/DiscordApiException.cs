using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Raw.Streaming.Discord.Exceptions;

[Serializable]
[ExcludeFromCodeCoverage]
public sealed class DiscordApiException : Exception
{
    private DiscordApiException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public DiscordApiException(string message) : base(message) { }
}