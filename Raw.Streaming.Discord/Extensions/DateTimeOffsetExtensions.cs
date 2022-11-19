using System;

namespace Raw.Streaming.Discord.Extensions
{
    internal static class DateTimeOffsetExtensions
    {
        public static string ToDiscordShortTime(this DateTimeOffset dateTime)
        {
            return $"<t:{dateTime.ToUnixTimeSeconds()}:t>";
        }
    }
}
