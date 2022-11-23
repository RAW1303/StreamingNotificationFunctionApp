using System;
using System.Runtime.Serialization;

namespace Raw.Streaming.Webhook.Exceptions
{
    [Serializable]
    public sealed class YouTubeApiException : Exception
    {
        private YouTubeApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public YouTubeApiException(string message) : base(message)
        {
        }
    }
}
