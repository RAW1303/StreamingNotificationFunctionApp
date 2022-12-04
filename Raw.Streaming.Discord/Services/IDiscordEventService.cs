using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services;

internal interface IDiscordEventService
{
    Task SyncScheduledEvents(string guildId, IEnumerable<Event> events);
    Task<IEnumerable<GuildScheduledEvent>> GetScheduledEvents(string guildId);
    Task<GuildScheduledEvent> CreateScheduledEvent(string guildId, GuildScheduledEvent guildScheduledEvent);
}
