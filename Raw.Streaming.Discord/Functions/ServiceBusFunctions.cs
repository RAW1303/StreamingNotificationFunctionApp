using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Discord.Model;
using Raw.Streaming.Discord.Services;
using System;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Functions
{
    [ServiceBusAccount("StreamingServiceBus")]
    internal class ServiceBusFunctions
    {
        private readonly IDiscordBotMessageService _discordBotMessageService;
        private readonly ILogger<ServiceBusFunctions> _logger;
        private readonly string _channelId = AppSettings.SandBoxChannelId;

        public ServiceBusFunctions(IDiscordBotMessageService discordBotMessageService, ILogger<ServiceBusFunctions> logger)
        {
            _logger = logger;
            _discordBotMessageService = discordBotMessageService;
        }

        [FunctionName(nameof(ProcessDiscordBotMessageQueue))]
        public async Task ProcessDiscordBotMessageQueue([ServiceBusTrigger("%DiscordBotMessageQueueName%")] DiscordBotQueueItem myQueueItem)
        {
            try
            {
                _logger.LogInformation($"Discord notification started");
                var message = await _discordBotMessageService.SendDiscordMessageAsync(_channelId, myQueueItem.Notification);
                _logger.LogInformation($"Discord notification succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Discord notification failed: {ex.Message}");
                throw;
            }
        }
    }
}
