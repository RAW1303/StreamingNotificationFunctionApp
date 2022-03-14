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
        private readonly ILogger<ServiceBusFunctions> _logger;
        private readonly IDiscordBotMessageService _discordBotMessageService;
        private readonly string _channelId = AppSettings.SandBoxChannelId;

        public ServiceBusFunctions(IDiscordBotMessageService discordBotMessageService)
        {
            _discordBotMessageService = discordBotMessageService;
        }

        [FunctionName(nameof(ProcessDiscordBotMessageQueue))]
        public async Task ProcessDiscordBotMessageQueue([ServiceBusTrigger("%DiscordBotMessageQueueName%")] DiscordBotQueueItem myQueueItem, ILogger logger)
        {
            try
            {
                logger.LogInformation($"Discord notification started");
                await _discordBotMessageService.SendDiscordMessageAsync(_channelId, myQueueItem.Message);
                logger.LogInformation($"Discord notification succeeded");
            }
            catch (Exception ex)
            {
                logger.LogError($"Discord notification failed: {ex.Message}");
                throw;
            }
        }
    }
}
