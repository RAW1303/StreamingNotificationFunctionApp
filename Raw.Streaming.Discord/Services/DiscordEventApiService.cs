using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services;

internal class DiscordEventApiService : DiscordApiService, IDiscordEventService
{
    public DiscordEventApiService(HttpClient httpClient, ILogger<DiscordEventApiService> logger) : base(httpClient, logger) { }

    public async Task SyncScheduledEventsAsync(string guildId, IEnumerable<Event> events)
    {
        var existingEvents = await GetScheduledEventsAsync(guildId);
        var botExistingEvents = existingEvents.Where(e => e.CreatorId == AppSettings.DiscordBotApplicationId);

        var eventsToAdd = EventToDiscordGuildScheduledEventTranslator.Translate(events.Where(x => !botExistingEvents.Any(y => x.Url == y.EntityMetadata.Location)));
        var eventsToUpdate = EventToDiscordGuildScheduledEventTranslator.Merge(botExistingEvents, events);
        var eventsToDelete = botExistingEvents.Where(x => !events.Any(y => x.EntityMetadata.Location == y.Url));

        var tasks = new List<Task>();
        tasks.AddRange(eventsToAdd.Select(x => CreateScheduledEventAsync(guildId, x)));
        tasks.AddRange(eventsToUpdate.Select(x => UpdateScheduledEventAsync(guildId, x.Id, x)));
        tasks.AddRange(eventsToDelete.Select(x => DeleteScheduledEventAsync(x.GuildId, x.Id)));

        await Task.WhenAll(tasks);
    }

    public async Task<IEnumerable<GuildScheduledEvent>> GetScheduledEventsAsync(string guildId)
    {
        try
        {
            var endpoint = $"guilds/{guildId}/scheduled-events";
            return await SendDiscordApiGetRequestAsync<IEnumerable<GuildScheduledEvent>>(endpoint);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Error getting scheduled events for guild {guildId}");
            throw;
        }
    }

    public async Task<GuildScheduledEvent> CreateScheduledEventAsync(string guildId, GuildScheduledEvent guildScheduledEvent)
    {
        try
        {
            var endpoint = $"guilds/{guildId}/scheduled-events";
            return await SendDiscordApiPostRequestAsync<GuildScheduledEvent>(endpoint, guildScheduledEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating scheduled event in guild {guildId}");
            throw;
        }
    }

    public async Task<GuildScheduledEvent> UpdateScheduledEventAsync(string guildId, string eventId, GuildScheduledEvent guildScheduledEvent)
    {
        try
        {
            var endpoint = $"guilds/{guildId}/scheduled-events/{eventId}";
            return await SendDiscordApiPatchRequestAsync<GuildScheduledEvent>(endpoint, guildScheduledEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating scheduled event {eventId} in guild {guildId}");
            throw;
        }
    }

    public async Task DeleteScheduledEventAsync(string guildId, string eventId)
    {
        try
        {
            var endpoint = $"guilds/{guildId}/scheduled-events/{eventId}";
            await SendDiscordApiDeleteRequestAsync(endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting scheduled event {eventId} in guild {guildId}");
            throw;
        }
    }
}
 