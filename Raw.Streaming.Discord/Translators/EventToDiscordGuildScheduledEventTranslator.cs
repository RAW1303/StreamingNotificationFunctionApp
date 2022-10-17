using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;
using System.Collections.Generic;
using System.Linq;

namespace Raw.Streaming.Discord.Translators
{
    internal static class EventToDiscordGuildScheduledEventTranslator
    {
        public static IEnumerable<GuildScheduledEvent> Translate(string eventIdParameterName, IEnumerable<Event> events)
        {
            return events.Select(x => Translate(eventIdParameterName, x));
        }

        public static GuildScheduledEvent Translate(string eventIdParameterName, Event eventModel)
        {
            var guildScheduledEvent = new GuildScheduledEvent()
            {
                Name = eventModel.Title,
                Description = $"{eventModel.Game}",
                ScheduledStartTime = eventModel.Start.DateTime,
                ScheduledEndTime = eventModel.End.DateTime,
                EntityMetadata = new GuildScheduledEventEntityMetadata() { Location = eventModel.Url }
            };

            guildScheduledEvent.AddDescriptionParameter(eventIdParameterName, eventModel.Id);
            return guildScheduledEvent;
        }
    }
}
