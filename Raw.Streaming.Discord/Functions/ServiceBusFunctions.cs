using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Services;
using Raw.Streaming.Discord.Translators;
using System;
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

        [FunctionName(nameof(ProcessGoLiveMessageQueue))]
        public async Task ProcessGoLiveMessageQueue([ServiceBusTrigger("%GoLiveQueueName%")] DiscordBotQueueItem<GoLive> myQueueItem)
        {
            try
            {
                _logger.LogInformation($"GoLive notification started");

                var messages = myQueueItem.Entities.Select(x => GoLiveToDiscordMessageTranslator.Translate(x));

                foreach (var message in messages)
                {
                    await _discordBotMessageService.SendDiscordMessageAsync(AppSettings.DiscordStreamGoLiveChannelId, message);
                }

                _logger.LogInformation($"GoLive notification succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError($"GoLive notification failed: {ex.Message}");
                throw;
            }
        }

        [FunctionName(nameof(ProcessClipMessageQueue))]
        public async Task ProcessClipMessageQueue([ServiceBusTrigger("%ClipsQueueName%")] DiscordBotQueueItem<Clip> myQueueItem)
        {
            try
            {
                _logger.LogInformation($"Clips notification started");

                var messages = myQueueItem.Entities.Select(x => ClipToDiscordMessageTranslator.Translate(x));

                foreach (var message in messages)
                {
                    await _discordBotMessageService.SendDiscordMessageAsync(AppSettings.DiscordClipChannelId, message);
                }

                _logger.LogInformation($"Clips notification succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Clips notification failed: {ex.Message}");
                throw;
            }
        }

        [FunctionName(nameof(ProcessVideoMessageQueue))]
        public async Task ProcessVideoMessageQueue([ServiceBusTrigger("%VideosQueueName%")] DiscordBotQueueItem<Video> myQueueItem)
        {
            try
            {
                _logger.LogInformation($"Videos notification started");

                var messages = myQueueItem.Entities.Select(x => VideoToDiscordMessageTranslator.Translate(x));

                foreach (var message in messages)
                {
                    await _discordBotMessageService.SendDiscordMessageAsync(AppSettings.DiscordVideoChannelId, message);
                }

                _logger.LogInformation($"Videos notification succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Videos notification failed: {ex.Message}");
                throw;
            }
        }

        [FunctionName(nameof(ProcessDailyScheduleMessageQueue))]
        public async Task ProcessDailyScheduleMessageQueue([ServiceBusTrigger("%DailyScheduleQueueName%")] DiscordBotQueueItem<Event> myQueueItem)
        {
            try
            {
                _logger.LogInformation($"DailySchedule notification started");

                var message = EventToDiscordMessageTranslator.TranslateDailySchedule(myQueueItem.Entities);

                await _discordBotMessageService.SendDiscordMessageAsync(AppSettings.DiscordScheduleChannelId, message);

                _logger.LogInformation($"DailySchedule notification succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError($"DailySchedule notification failed: {ex.Message}");
                throw;
            }
        }

        [FunctionName(nameof(ProcessWeeklyScheduleMessageQueue))]
        public async Task ProcessWeeklyScheduleMessageQueue([ServiceBusTrigger("%WeeklyScheduleQueueName%")] DiscordBotQueueItem<Event> myQueueItem)
        {
            try
            {
                _logger.LogInformation($"WeeklySchedule notification started");

                var message = EventToDiscordMessageTranslator.TranslateWeeklySchedule(myQueueItem.Entities);

                await _discordBotMessageService.SendDiscordMessageAsync(AppSettings.DiscordScheduleChannelId, message);

                _logger.LogInformation($"WeeklySchedule notification succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError($"WeeklySchedule notification failed: {ex.Message}");
                throw;
            }
        }

        [FunctionName(nameof(ProcessEventMessageQueue))]
        public async Task ProcessEventMessageQueue([ServiceBusTrigger("%EventQueueName%")] DiscordBotQueueItem<Event> myQueueItem)
        {
            try
            {
                _logger.LogInformation($"{nameof(ProcessEventMessageQueue)} notification started");

                var message = EventToDiscordMessageTranslator.TranslateWeeklySchedule(myQueueItem.Entities);

                await _discordBotMessageService.SendDiscordMessageAsync(AppSettings.DiscordScheduleChannelId, message);

                _logger.LogInformation($"{nameof(ProcessEventMessageQueue)} notification succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ProcessEventMessageQueue)} notification failed: {ex.Message}");
                throw;
            }
        }
    }
}
