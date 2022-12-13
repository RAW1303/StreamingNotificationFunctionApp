using Microsoft.Extensions.Logging;
using Raw.Streaming.Discord.Model.DiscordApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services;

internal class DiscordEventApiService : BaseDiscordApiService, IDiscordEventService
{
    public DiscordEventApiService(HttpClient httpClient, ILogger<DiscordEventApiService> logger) : base(httpClient, logger) { }

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
 