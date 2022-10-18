using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services;

internal class DiscordEventService : IDiscordEventService
{
    private readonly IDiscordApiService _discordApiService;
    private readonly ILogger _logger;

    public DiscordEventService(IDiscordApiService discordApiService, ILogger<DiscordEventService> logger)
    {
        _discordApiService = discordApiService;
        _logger = logger;
    }

    public async Task<IEnumerable<GuildScheduledEvent>> SyncScheduledEvents(string guildId, IEnumerable<Event> events)
    {
        var channelOverride = AppSettings.EventChannelOverride;
        var isChannelOverriden = string.IsNullOrEmpty(channelOverride);
        var sourceIdParameterName = "SourceId";
        var existingEvents = await GetScheduledEvents(guildId);
        var filteredEvents = FilterEvents(existingEvents, channelOverride);
        var eventsToAdd = EventToDiscordGuildScheduledEventTranslator.Translate(sourceIdParameterName, events.Where(x => !filteredEvents.Any(y => y.HasDescriptionParameter(sourceIdParameterName, x.Id))), channelOverride);
        var tasks = eventsToAdd.Select(x => CreateScheduledEvent(guildId, x));
        return await Task.WhenAll(tasks);
    }

    public async Task<IEnumerable<GuildScheduledEvent>> GetScheduledEvents(string guildId)
    {
        try
        {
            var endpoint = $"guilds/{guildId}/scheduled-events";
            return await _discordApiService.SendDiscordApiGetRequestAsync<IEnumerable<GuildScheduledEvent>>(endpoint);
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
            return await _discordApiService.SendDiscordApiPostRequestAsync<GuildScheduledEvent>(endpoint, guildScheduledEvent);
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
            return await _discordApiService.SendDiscordApiPatchRequestAsync<GuildScheduledEvent>(endpoint, guildScheduledEvent);
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
            await _discordApiService.SendDiscordApiDeleteRequestAsync(endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error delete scheduled event {eventId} in guild {guildId}");
            throw;
        }
    }

    private IEnumerable<GuildScheduledEvent> FilterEvents(IEnumerable<GuildScheduledEvent> events, string overrideChannel)
    {
        if (string.IsNullOrEmpty(overrideChannel))
            return events;

        return events.Where(e => e.ChannelId == overrideChannel);
    }
}
