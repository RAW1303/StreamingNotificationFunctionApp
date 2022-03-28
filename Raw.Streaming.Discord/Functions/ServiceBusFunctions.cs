using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model.Enums;
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

                foreach (var message in myQueueItem.Messages)
                {
                    await _discordBotMessageService.SendDiscordMessageAsync(ResolveChannelId(myQueueItem.Type), message);
                }

                _logger.LogInformation($"Discord notification succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Discord notification failed: {ex.Message}");
                throw;
            }
        }

        private string ResolveChannelId(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.StreamGoLive => AppSettings.DiscordStreamGoLiveChannelId,
                MessageType.Clip => AppSettings.DiscordClipChannelId,
                MessageType.Video => AppSettings.DiscordVideoChannelId,
                MessageType.Schedule => AppSettings.DiscordScheduleChannelId,
                _ => throw new ArgumentOutOfRangeException(nameof(messageType), $"Not expected messageType value: {messageType}"),
            };
        }
    }
}
