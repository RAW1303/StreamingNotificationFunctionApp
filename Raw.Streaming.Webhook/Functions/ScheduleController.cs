using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Model;
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
        public async Task NotifyWeeklySchedule(
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
                await _discordNotificationService.SendNotification(_discordwebhookId, _discordwebhookToken, notification);
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(NotifyWeeklySchedule)} execution failed: {e.Message}");
                throw;
            }
        }


        [FunctionName("NotifyDailySchedule")]
        [return: ServiceBus("discordnotificationqueue", Connection = "StreamingServiceBus")]
        public async Task NotifyDailySchedule(
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
                    await _discordNotificationService.SendNotification(_discordwebhookId, _discordwebhookToken, notification);
                }
            }
            catch (Exception e)
            {
                logger.LogError($"{nameof(NotifyWeeklySchedule)} execution failed: {e.Message}");
                throw;
            }
        }
    }
}
