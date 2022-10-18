using System;

namespace Raw.Streaming.Discord.Model.DiscordApi.Exceptions;
internal class DiscordApiRateLimitException : Exception
{
    public DiscordApiRateLimitException() : base() { }
    public DiscordApiRateLimitException(string message) : base(message) { }
}
