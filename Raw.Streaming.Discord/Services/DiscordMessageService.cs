using Microsoft.Extensions.Logging;
using Raw.Streaming.Discord.Model.DiscordApi;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services
{
    internal class DiscordMessageService : BaseDiscordApiService, IDiscordMessageService
    {
        public DiscordMessageService(ILogger<DiscordMessageService> logger, HttpClient httpClient) : base(logger, httpClient)
        {
        }

        public async Task<Message> SendDiscordMessageAsync(string channelId, Message message)
        {
            try
            {
                var endpoint = $"/channels/{channelId}/messages";
                var response = await SendDiscordApiPostRequestAsync(endpoint, message);
                var responseObject = JsonSerializer.Deserialize<Message>(await response.Content.ReadAsStringAsync());
                return responseObject;
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
                var response = await SendDiscordApiPostRequestAsync(endpoint);
                var responseObject = JsonSerializer.Deserialize<Message>(await response.Content.ReadAsStringAsync());
                return responseObject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error crossposting message {messageId} in channel {channelId}");
                throw;
            }
        }
    }
}
