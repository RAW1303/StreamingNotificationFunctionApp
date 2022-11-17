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
        [FunctionName(nameof(UpdateEventScheduleTrigger))]
        [return: ServiceBus("%EventScheduleQueueName%")]
        public async Task<ServiceBusMessage> UpdateEventScheduleTrigger(
            [TimerTrigger("%EventScheduleTimerTrigger%")] TimerInfo timer)
        {
            return await UpdateEventSchedule(DateTimeOffset.UtcNow);
        }

        public async Task<ServiceBusMessage> UpdateEventSchedule(DateTimeOffset triggerTime)
        {
            try
            {
                _logger.LogDebug($"{nameof(UpdateEventSchedule)} execution started for {triggerTime}");
                var events =  await _scheduleService.GetScheduleAsync(triggerTime);
                var filteredEvents = events.Where(x => !x.IsRecurring || x.Start < triggerTime.AddDays(7));
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
