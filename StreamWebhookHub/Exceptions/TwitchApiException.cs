using System;

namespace Raw.Streaming.Webhook.Exceptions
{
    public class TwitchApiException : Exception
    {
        public TwitchApiException(string message) : base(message)
        {
        }
    }
}
