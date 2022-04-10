using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Common.Model.Enums;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Services;
using Raw.Streaming.Discord.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
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

                var messages = TranslateEntities(myQueueItem.Type, myQueueItem.Entities);

                foreach (var message in messages)
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

        private IEnumerable<Message> TranslateEntities(MessageType messageType, IEnumerable<Entity> entities)
        {
            IEnumerable<Message> messages = messageType switch
            {
                MessageType.StreamGoLive => entities.Select(x => GoLiveToDiscordMessageTranslator.Translate(x as GoLive)),
                MessageType.Clip => entities.Select(x => TwitchClipToDiscordMessageTranslator.Translate(x as Clip)),
                MessageType.Video => entities.Select(x => VideoToDiscordMessageTranslator.Translate(x as Video)),
                MessageType.DailySchedule => new Message[] { EventToDiscordMessageTranslator.TranslateDailySchedule(entities as IEnumerable<Event>) },
                MessageType.WeeklySchedule => new Message[] { EventToDiscordMessageTranslator.TranslateWeeklySchedule(entities as IEnumerable<Event>) },
                _ => throw new ArgumentOutOfRangeException(nameof(messageType), $"Not expected messageType value: {messageType}"),
            };

            return messages.Where(x => x != null);
        }

        private static string ResolveChannelId(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.StreamGoLive => AppSettings.DiscordStreamGoLiveChannelId,
                MessageType.Clip => AppSettings.DiscordClipChannelId,
                MessageType.Video => AppSettings.DiscordVideoChannelId,
                MessageType.DailySchedule => AppSettings.DiscordScheduleChannelId,
                MessageType.WeeklySchedule => AppSettings.DiscordScheduleChannelId,
                _ => throw new ArgumentOutOfRangeException(nameof(messageType), $"Not expected messageType value: {messageType}"),
            };
        }
    }
}
