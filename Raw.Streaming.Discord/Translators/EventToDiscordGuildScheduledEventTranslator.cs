using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Raw.Streaming.Discord.Translators
{
    internal static class EventToDiscordGuildScheduledEventTranslator
    {
        public static IEnumerable<GuildScheduledEvent> Translate(IEnumerable<Event> events)
        {
            return events.Select(x => Translate(x));
        }

        public static GuildScheduledEvent Translate(Event eventModel)
        {
            var guildScheduledEvent = new GuildScheduledEvent()
            {
                Name = eventModel.Title,
                Description = $"{eventModel.Description}",
                ScheduledStartTime = eventModel.Start.DateTime,
                ScheduledEndTime = eventModel.End?.DateTime ?? eventModel.Start.DateTime.AddHours(3),
                Status = GuildScheduledEventStatus.SCHEDULED,
                PrivacyLevel = GuildScheduledEventPrivacyLevel.GUILD_ONLY,
                EntityType = GuildScheduledEventEntityType.EXTERNAL,
                EntityMetadata = new GuildScheduledEventEntityMetadata() { Location = eventModel.Url }
            };

            return guildScheduledEvent;
        }

        public static bool IsUpdate(GuildScheduledEvent target, Event source)
        {
            return source.Url == target.EntityMetadata.Location
                && (source.Title != target.Name
                || source.Description != target.Description
                || source.Start.DateTime != target.ScheduledStartTime
                || source.End?.DateTime != target.ScheduledEndTime);
        }
    }
}
