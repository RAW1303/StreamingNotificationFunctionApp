using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Extensions;
using Raw.Streaming.Discord.Model.DiscordApi;
using System.Collections.Generic;
using System.Linq;

namespace Raw.Streaming.Discord.Translators
{
    internal static class SheduledEventToDiscordMessageTranslator
    {
        public static Message TranslateDailySchedule(IEnumerable<GuildScheduledEvent> events)
        {
            return new Message()
            {
                Content = string.Join('\n', events.Select(x => $"https://discord.com/events/{x.GuildId}/{x.Id}"))
            };
        }
    }
}
