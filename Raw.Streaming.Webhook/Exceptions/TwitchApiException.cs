using System;
using System.Runtime.Serialization;

namespace Raw.Streaming.Webhook.Exceptions
{
    [Serializable]
    public sealed class TwitchApiException : Exception
    {
        private TwitchApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public TwitchApiException(string message) : base(message)
        {
        }
    }
}
