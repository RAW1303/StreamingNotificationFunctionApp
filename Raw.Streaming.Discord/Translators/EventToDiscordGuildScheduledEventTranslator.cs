using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;
using System.Collections.Generic;
using System.Linq;

namespace Raw.Streaming.Discord.Translators
{
    internal static class EventToDiscordGuildScheduledEventTranslator
    {
        private static readonly string _channelOverride = AppSettings.EventChannelOverride;

        public static IEnumerable<GuildScheduledEvent> Translate(IEnumerable<Event> events)
        {
            return events.Select(x => Translate(x));
        }

        public static GuildScheduledEvent Translate(Event eventModel)
        {
            var isChannelOverriden = string.IsNullOrEmpty(_channelOverride);
            var guildScheduledEvent = new GuildScheduledEvent()
            {
                Name = eventModel.Title,
                Description = $"{eventModel.Description}",
                ScheduledStartTime = eventModel.Start.DateTime,
                ScheduledEndTime = eventModel.End?.DateTime ?? eventModel.Start.DateTime.AddHours(3),
                Status = GuildScheduledEventStatus.SCHEDULED,
                PrivacyLevel = GuildScheduledEventPrivacyLevel.GUILD_ONLY,
                EntityType = isChannelOverriden ? GuildScheduledEventEntityType.EXTERNAL : GuildScheduledEventEntityType.VOICE,
                ChannelId = _channelOverride,
                EntityMetadata = isChannelOverriden ? new GuildScheduledEventEntityMetadata() { Location = eventModel.Url } : null
            };

            return guildScheduledEvent;
        }
    }
}
