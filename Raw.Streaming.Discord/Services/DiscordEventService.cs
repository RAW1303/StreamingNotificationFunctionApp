using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services;

internal class DiscordEventService : BaseDiscordApiService, IDiscordEventService
{
    public DiscordEventService(ILogger<DiscordEventService> logger, HttpClient httpClient) : base(logger, httpClient)
    {
    }

    public async Task<IEnumerable<GuildScheduledEvent>> SyncScheduledEvents(string guildId, IEnumerable<Event> events)
    {
        var sourceIdParameterName = "SourceId";
        var existingEvents = await GetScheduledEvents(guildId);
        var eventsToAdd = EventToDiscordGuildScheduledEventTranslator.Translate(sourceIdParameterName, events.Where(x => !existingEvents.Any(y => y.HasDescriptionParameter(sourceIdParameterName, x.Id))));
        var tasks = eventsToAdd.Select(x => CreateScheduledEvent(guildId, x));
        return await Task.WhenAll(tasks);
    }

    public async Task<IEnumerable<GuildScheduledEvent>> GetScheduledEvents(string guildId)
    {
        try
        {
            var endpoint = $"guilds/{guildId}/scheduled-events";
            var response = await SendDiscordApiGetRequestAsync(endpoint);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<IEnumerable<GuildScheduledEvent>>(jsonContent);
            return responseObject;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Error getting scheduled events for guild {guildId}");
            throw;
        }
    }

    public async Task<GuildScheduledEvent> CreateScheduledEvent(string guildId, GuildScheduledEvent guildScheduledEvent)
    {
        try
        {
            var endpoint = $"guilds/{guildId}/scheduled-events";
            var response = await SendDiscordApiPostRequestAsync(endpoint, guildScheduledEvent);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<GuildScheduledEvent>(jsonContent);
            return responseObject;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating scheduled event in guild {guildId}");
            throw;
        }
    }

    public async Task<GuildScheduledEvent> UpdateScheduledEvent(string guildId, string eventId, GuildScheduledEvent guildScheduledEvent)
    {
        try
        {
            var endpoint = $"guilds/{guildId}/scheduled-events/{eventId}";
            var response = await SendDiscordApiPatchRequestAsync(endpoint, guildScheduledEvent);
            var jsonContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<GuildScheduledEvent>(jsonContent);
            return responseObject;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating scheduled event {eventId} in guild {guildId}");
            throw;
        }
    }

    public async Task DeleteScheduledEvent(string guildId, string eventId)
    {
        try
        {
            var endpoint = $"guilds/{guildId}/scheduled-events/{eventId}";
            var response = await SendDiscordApiPatchRequestAsync(endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error delete scheduled event {eventId} in guild {guildId}");
            throw;
        }
    }
}
