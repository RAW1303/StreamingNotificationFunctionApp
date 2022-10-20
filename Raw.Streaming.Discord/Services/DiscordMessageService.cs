using Microsoft.Extensions.Logging;
using Raw.Streaming.Discord.Model.DiscordApi;
using System;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services
{
    internal class DiscordMessageService : IDiscordMessageService
    {
        private readonly IDiscordApiService _discordApiService;
        private readonly ILogger _logger;

        public DiscordMessageService(IDiscordApiService discordApiService, ILogger<DiscordMessageService> logger)
        {
            _discordApiService = discordApiService;
            _logger = logger;
        }

        public async Task<Message> SendDiscordMessageAsync(string channelId, Message message)
        {
            try
            {
                var endpoint = $"/channels/{channelId}/messages";
                return await _discordApiService.SendDiscordApiPostRequestAsync<Message>(endpoint, message);
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
                return await _discordApiService.SendDiscordApiPostRequestAsync<Message>(endpoint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error crossposting message {messageId} in channel {channelId}");
                throw;
            }
        }
    }
}
