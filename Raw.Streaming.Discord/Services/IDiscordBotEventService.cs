using Raw.Streaming.Discord.Model.DiscordApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services
{
    internal interface IDiscordBotEventService
    {
        Task<IEnumerable<GuildScheduledEvent>> GetScheduledEvents(string guildId);
        Task<GuildScheduledEvent> CreateScheduledEvent(string guildId, GuildScheduledEvent guildScheduledEvent);
        Task<GuildScheduledEvent> UpdateScheduledEvent(string guildId, string eventId, GuildScheduledEvent guildScheduledEvent);
        Task DeleteScheduledEvent(string guildId, string eventId);
    }
}
