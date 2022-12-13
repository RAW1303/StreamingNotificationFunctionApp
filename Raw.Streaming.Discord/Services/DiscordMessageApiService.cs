using Microsoft.Extensions.Logging;
using Raw.Streaming.Discord.Model.DiscordApi;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services;

internal class DiscordMessageApiService : DiscordApiService, IDiscordMessageService
{
    public DiscordMessageApiService(HttpClient httpClient, ILogger<DiscordMessageApiService> logger) : base(httpClient, logger) { }

    public async Task<Message> SendDiscordMessageAsync(string channelId, Message message)
    {
        try
        {
            var endpoint = $"/channels/{channelId}/messages";
            return await SendDiscordApiPostRequestAsync<Message>(endpoint, message);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"Error sending message to channel {channelId}");
            throw;
        }
    }

    public async Task<Message> CrosspostDiscordMessageAsync(string channelId, string messageId)
    {
        try
        {
            var endpoint = $"/channels/{channelId}/messages/{messageId}/crosspost";
            return await SendDiscordApiPostRequestAsync<Message>(endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error crossposting message {messageId} in channel {channelId}");
            throw;
        }
    }
}
