using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;
using System.Collections.Generic;
using System.Linq;

namespace Raw.Streaming.Discord.Translators
{
    internal static class EventToDiscordGuildScheduledEventTranslator
    {
        public static IEnumerable<GuildScheduledEvent> Translate(string eventIdParameterName, IEnumerable<Event> events, string channelOverride = null)
        {
            return events.Select(x => Translate(eventIdParameterName, x, channelOverride));
        }

        public static GuildScheduledEvent Translate(string eventIdParameterName, Event eventModel, string channelOverride = null)
        {
            var isChannelOverriden = string.IsNullOrEmpty(channelOverride);
            var guildScheduledEvent = new GuildScheduledEvent()
            {
                Name = eventModel.Title,
                Description = $"{eventModel.Game}",
                ScheduledStartTime = eventModel.Start.DateTime,
                ScheduledEndTime = eventModel.End.DateTime,
                Status = GuildScheduledEventStatus.SCHEDULED,
                PrivacyLevel = GuildScheduledEventPrivacyLevel.GUILD_ONLY,
                EntityType = isChannelOverriden ? GuildScheduledEventEntityType.EXTERNAL : GuildScheduledEventEntityType.VOICE,
                ChannelId = channelOverride,
                EntityMetadata = isChannelOverriden ? new GuildScheduledEventEntityMetadata() { Location = eventModel.Url } : null
            };

            guildScheduledEvent.AddDescriptionParameter(eventIdParameterName, eventModel.Id);
            return guildScheduledEvent;
        }
    }
}
