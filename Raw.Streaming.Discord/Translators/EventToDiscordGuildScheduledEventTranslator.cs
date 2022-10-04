using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;
using System.Collections.Generic;

namespace Raw.Streaming.Discord.Translators
{
    internal static class EventToDiscordGuildScheduledEventTranslator
    {
        public static IEnumerable<GuildScheduledEvent> Translate(IEnumerable<Event> events)
        {
            return new List<GuildScheduledEvent>();
        }

        public static GuildScheduledEvent Translate(Event eventModel)
        {
            return new GuildScheduledEvent()
            {
                Name = eventModel.Title,
                Description = $"{eventModel.Game}\n{eventModel.Id}",
                ScheduledStartTime = eventModel.Start.DateTime,
                ScheduledEndTime = eventModel.End.DateTime,
                EntityMetadata = new GuildScheduledEventEntityMetadata() { Location = eventModel.Url }
            };
        }
    }
}
