using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Webhook.Services;

namespace Raw.Streaming.Webhook.Functions
{
    [ServiceBusAccount("StreamingServiceBus")]
    internal class ScheduleController
    {
        private readonly IScheduleService _scheduleService;
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(
            IScheduleService scheduleService,
            ILogger<ScheduleController> logger)
        {
            _scheduleService = scheduleService;
            _logger = logger;
        }

        [ExcludeFromCodeCoverage]
        [FunctionName(nameof(NotifyDailyScheduleTrigger))]
        [return: ServiceBus("%DailyScheduleQueueName%")]
        public async Task<ServiceBusMessage> NotifyDailyScheduleTrigger(
            [TimerTrigger("%ScheduleDailyTimerTrigger%")] TimerInfo timer)
        {
            return await NotifyDailySchedule(DateTimeOffset.UtcNow);
        }

        [ExcludeFromCodeCoverage]
        [FunctionName(nameof(NotifyWeeklyScheduleTrigger))]
        [return: ServiceBus("%WeeklyScheduleQueueName%")]
        public async Task<ServiceBusMessage> NotifyWeeklyScheduleTrigger(
            [TimerTrigger("%ScheduleWeeklyTimerTrigger%")] TimerInfo timer)
        {
            return await NotifyWeeklySchedule(DateTimeOffset.UtcNow);
        }

        [ExcludeFromCodeCoverage]
        [FunctionName(nameof(UpdateEventScheduleTrigger))]
        [return: ServiceBus("%EventScheduleQueueName%")]
        public async Task<ServiceBusMessage> UpdateEventScheduleTrigger(
            [TimerTrigger("%EventScheduleTimerTrigger%")] TimerInfo timer)
        {
            return await UpdateEventSchedule(DateTimeOffset.UtcNow);
        }

        public async Task<ServiceBusMessage> NotifyWeeklySchedule(DateTimeOffset triggerTime)
        {
            try
            {
                _logger.LogDebug($"{nameof(NotifyWeeklySchedule)} execution started for {triggerTime}");
                var from = triggerTime;
                var to = from.AddDays(7);
                var events = await _scheduleService.GetScheduleAsync(triggerTime, to);
                var queueItem = new DiscordBotQueueItem<Event>(events.ToArray());
                return new ServiceBusMessage
                {
                    Body = BinaryData.FromObjectAsJson(queueItem),
                    MessageId = $"schedule-weekly-{from:yyyy-MM-dd}"
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(NotifyWeeklySchedule)} execution failed: {e.Message}");
                throw;
            }
        }

        public async Task<ServiceBusMessage> NotifyDailySchedule(DateTimeOffset triggerTime)
        {
            try
            {
                _logger.LogDebug($"{nameof(NotifyDailySchedule)} execution started for {triggerTime}");
                var from = triggerTime.Date;
                var to = from.AddDays(1);
                var events = await _scheduleService.GetScheduleAsync(triggerTime, to);
                if (events.Any())
                {
                    var queueItem = new DiscordBotQueueItem<Event>(events.ToArray());
                    return new ServiceBusMessage
                    {
                        Body = BinaryData.FromObjectAsJson(queueItem),
                        MessageId = $"schedule-daily-{from:yyyy-MM-dd}"
                    };
                }

                return null;
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(NotifyDailySchedule)} execution failed: {e.Message}");
                throw;
            }

        }

        public async Task<ServiceBusMessage> UpdateEventSchedule(DateTimeOffset triggerTime)
        {
            try
            {
                _logger.LogDebug($"{nameof(UpdateEventSchedule)} execution started for {triggerTime}");
                var events =  await _scheduleService.GetScheduleAsync(triggerTime);
                var filteredEvents = events.Where(x => !x.IsRecurring);
                var queueItem = new DiscordBotQueueItem<Event>(filteredEvents.ToArray());
                return new ServiceBusMessage
                {
                    Body = BinaryData.FromObjectAsJson(queueItem),
                    MessageId = $"event-schedule-{triggerTime:yyyy-MM-ddTHH:mm:ss}"
                };
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(UpdateEventSchedule)} execution failed: {e.Message}");
                throw;
            }

        }
    }
}
