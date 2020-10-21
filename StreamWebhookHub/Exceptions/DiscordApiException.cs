using System;

namespace Raw.Streaming.Webhook.Exceptions
{
    public class DiscordApiException : Exception
    {
        public DiscordApiException(string message) : base(message)
        {
        }
    }
}