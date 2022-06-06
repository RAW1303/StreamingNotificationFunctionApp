using System;

namespace Raw.Streaming.Discord.Extensions
{
    internal static class DateTimeOffsetExtensions
    {
        public static string ToDiscordShortTime(this DateTimeOffset dateTime)
        {
            return $"<t:{dateTime.ToUnixTimeSeconds}:t>";
        }
        public static string ToDiscordLongTime(this DateTimeOffset dateTime)
        {
            return $"<t:{dateTime.ToUnixTimeSeconds}:T>";
        }
        public static string ToDiscordShortDate(this DateTimeOffset dateTime)
        {
            return $"<t:{dateTime.ToUnixTimeSeconds}:d>";
        }
        public static string ToDiscordLongDate(this DateTimeOffset dateTime)
        {
            return $"<t:{dateTime.ToUnixTimeSeconds}:D>";
        }
        public static string ToDiscordShortDateTime(this DateTimeOffset dateTime)
        {
            return $"<t:{dateTime.ToUnixTimeSeconds}:f>";
        }
        public static string ToDiscordLongDateTime(this DateTimeOffset dateTime)
        {
            return $"<t:{dateTime.ToUnixTimeSeconds}:F>";
        }
        public static string ToDiscordRelaitveDateTime(this DateTimeOffset dateTime)
        {
            return $"<t:{dateTime.ToUnixTimeSeconds}:R>";
        }
    }
}
