using Raw.Streaming.Common.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services;
internal interface IEventManagementService
{
    Task SyncScheduledEventsAsync(string guildId, IEnumerable<Event> events);
}
