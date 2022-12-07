using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;
using System.Collections.Generic;
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
                Name = eventModel.Title ?? string.Empty,
                Description = eventModel.Description ?? string.Empty,
                ScheduledStartTime = eventModel.Start.DateTime,
                ScheduledEndTime = eventModel.End?.DateTime ?? eventModel.Start.DateTime.AddHours(3),
                Status = GuildScheduledEventStatus.SCHEDULED,
                PrivacyLevel = GuildScheduledEventPrivacyLevel.GUILD_ONLY,
                EntityType = GuildScheduledEventEntityType.EXTERNAL,
                EntityMetadata = new GuildScheduledEventEntityMetadata() { Location = eventModel.Url ?? string.Empty }
            };

            return guildScheduledEvent;
        }

        public static IEnumerable<GuildScheduledEvent> Merge(IEnumerable<GuildScheduledEvent> targets, IEnumerable<Event> sources)
        {
            foreach(var target in targets)
            {
                var source = sources.FirstOrDefault(x => IsUpdate(target, x));

                if (source is null) continue;

                yield return Merge(target, source);
            }
        }

        public static GuildScheduledEvent Merge(GuildScheduledEvent target, Event source)
        {
            target.Name = source.Title ?? string.Empty;
            target.Description = source.Description ?? string.Empty;
            target.ScheduledStartTime = source.Start.DateTime;
            target.ScheduledEndTime = source.End?.DateTime ?? target.ScheduledEndTime;
            return target;
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
