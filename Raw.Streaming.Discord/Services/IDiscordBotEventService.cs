using Raw.Streaming.Discord.Model.DiscordApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services
{
    internal interface IDiscordBotEventService
    {
        Task<IEnumerable<GuildScheduledEvent>> GetScheduledEvents();
        Task<GuildScheduledEvent> GetScheduledEvent(GuildScheduledEvent guildScheduledEvent);
    }
}
