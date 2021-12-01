using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Model.Discord;
using Raw.Streaming.Webhook.Services;
using Raw.Streaming.Webhook.Translators;

namespace Raw.Streaming.Webhook.Functions
{
    public class ScheduleController
    {
        private readonly string _discordwebhookId = AppSettings.DiscordScheduleLiveWebhookId;
        private readonly string _discordwebhookToken = AppSettings.DiscordScheduleLiveWebhookToken;
        private readonly IScheduleService _scheduleService;
        private readonly ScheduledStreamToDiscordNotificationTranslator _translator;
        private readonly IDiscordNotificationService _discordNotificationService;

        public ScheduleController(
            IScheduleService scheduleService,
            IDiscordNotificationService discordNotificationService,
            ScheduledStreamToDiscordNotificationTranslator translator)
        {
            _scheduleService = scheduleService;
            _discordNotificationService = discordNotificationService;
            _translator = translator;
        }


        [FunctionName("NotifyWeeklySchedule")]
        [return: ServiceBus("discordnotificationqueue", Connection = "StreamingServiceBus")]
        public async Task<ServiceBusMessage> NotifyWeeklySchedule(
            [TimerTrigger("%ScheduleWeeklyTimerTrigger%")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                logger.LogInformation($"{nameof(NotifyWeeklySchedule)} execution started");
                var from = DateTime.Today;
                var to = from.AddDays(7);
                var scheduledStreams = await _scheduleService.GetScheduledStreamsAsync(from, to);
                var notification = _translator.TranslateWeeklySchedule(scheduledStreams);
                var message = new DiscordMessage(_discordwebhookId, _discordwebhookToken, notification);
                return new ServiceBusMessage
                {
                    Body = BinaryData.FromObjectAsJson(message),
                    MessageId = $"schedule-weekly-{from:yyyy-MM-dd}"
                };
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(NotifyWeeklySchedule)} execution failed: {e.Message}");
                throw;
            }
        }


        [FunctionName("NotifyDailySchedule")]
        [return: ServiceBus("discordnotificationqueue", Connection = "StreamingServiceBus")]
        public async Task<ServiceBusMessage> NotifyDailySchedule(
            [TimerTrigger("%ScheduleDailyTimerTrigger%")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                var from = DateTime.Today;
                var to = from.AddDays(1);
                logger.LogInformation($"{nameof(NotifyWeeklySchedule)} execution started for {from:d}");
                var scheduledStreams = await _scheduleService.GetScheduledStreamsAsync(from, to);
                if (scheduledStreams.Count > 0)
                {
                    var notification = _translator.TranslateDailySchedule(scheduledStreams);
                    var message = new DiscordMessage(_discordwebhookId, _discordwebhookToken, notification);
                    return new ServiceBusMessage
                    {
                        Body = BinaryData.FromObjectAsJson(message),
                        MessageId = $"schedule-daily-{from:yyyy-MM-dd}"
                    };
                }

                return null;
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(NotifyWeeklySchedule)} execution failed: {e.Message}");
                throw;
            }
        }
    }
}
