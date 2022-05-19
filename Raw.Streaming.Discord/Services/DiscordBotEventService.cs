using Raw.Streaming.Discord.Model.DiscordApi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services
{
    internal class DiscordBotEventService : IDiscordBotEventService
    {
        public Task<GuildScheduledEvent> GetScheduledEvent(GuildScheduledEvent guildScheduledEvent)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<GuildScheduledEvent>> GetScheduledEvents()
        {
            throw new System.NotImplementedException();
        }
    }
}
