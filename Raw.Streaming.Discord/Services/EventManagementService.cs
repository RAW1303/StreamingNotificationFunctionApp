using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Translators;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services;
internal class EventManagementService : IEventManagementService
{
    private readonly ILogger<EventManagementService> _logger;
    private readonly IDiscordEventService _discordEventService;

    public EventManagementService(IDiscordEventService discordEventService, ILogger<EventManagementService> logger)
    {
        _discordEventService = discordEventService;
        _logger = logger;
    }

    public async Task SyncScheduledEventsAsync(string guildId, IEnumerable<Event> events)
    {
        var existingEvents = await _discordEventService.GetScheduledEventsAsync(guildId);
        var botExistingEvents = existingEvents.Where(e => e.CreatorId == AppSettings.DiscordBotApplicationId);

        var eventsToAdd = EventToDiscordGuildScheduledEventTranslator.Translate(events.Where(x => !botExistingEvents.Any(y => x.Url == y.EntityMetadata?.Location)));
        var eventsToUpdate = EventToDiscordGuildScheduledEventTranslator.Merge(botExistingEvents, events);
        var eventsToDelete = botExistingEvents.Where(x => !events.Any(y => x.EntityMetadata?.Location == y.Url));

        var tasks = new List<Task>();
        tasks.AddRange(eventsToAdd.Select(x => _discordEventService.CreateScheduledEventAsync(guildId, x)));
        tasks.AddRange(eventsToUpdate.Select(x => _discordEventService.UpdateScheduledEventAsync(guildId, x.Id, x)));
        tasks.AddRange(eventsToDelete.Select(x => _discordEventService.DeleteScheduledEventAsync(guildId, x.Id)));

        await Task.WhenAll(tasks);
    }
}
