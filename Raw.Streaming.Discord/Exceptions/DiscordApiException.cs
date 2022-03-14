﻿using System;
using System.Runtime.Serialization;

namespace Raw.Streaming.Discord.Exceptions
{
    [Serializable]
    public sealed class DiscordApiException : Exception
    {
        private DiscordApiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DiscordApiException(string message) : base(message)
        {
        }
    }
}