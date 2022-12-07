using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services;

internal interface IDiscordEventService
{
    Task SyncScheduledEventsAsync(string guildId, IEnumerable<Event> events);
    Task<IEnumerable<GuildScheduledEvent>> GetScheduledEventsAsync(string guildId);
    Task<GuildScheduledEvent> CreateScheduledEventAsync(string guildId, GuildScheduledEvent guildScheduledEvent);
    Task<GuildScheduledEvent> UpdateScheduledEventAsync(string guildId, string eventId, GuildScheduledEvent guildScheduledEvent);
    Task DeleteScheduledEventAsync(string guildId, string eventId);
}
