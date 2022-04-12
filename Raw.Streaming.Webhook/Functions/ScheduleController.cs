using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Webhook.Services;

namespace Raw.Streaming.Webhook.Functions
{
    [ServiceBusAccount("StreamingServiceBus")]
    public class ScheduleController
    {
        private readonly IScheduleService _scheduleService;
        private readonly IMapper _mapper;

        public ScheduleController(
            IScheduleService scheduleService,
            IMapper mapper)
        {
            _scheduleService = scheduleService;
            _mapper = mapper;
        }


        [FunctionName("NotifyWeeklySchedule")]
        [return: ServiceBus("%WeeklyScheduleQueueName%")]
        public async Task<ServiceBusMessage> NotifyWeeklySchedule(
            [TimerTrigger("%ScheduleWeeklyTimerTrigger%")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                logger.LogInformation($"{nameof(NotifyWeeklySchedule)} execution started");
                var from = DateTime.Today;
                var to = from.AddDays(7);
                var streamEvents = await _scheduleService.GetScheduledStreamsAsync(from, to);
                var events = _mapper.Map<IEnumerable<Event>>(streamEvents);
                var queueItem = new DiscordBotQueueItem<Event>(events.ToArray());
                return new ServiceBusMessage
                {
                    Body = BinaryData.FromObjectAsJson(queueItem),
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
        [return: ServiceBus("%DailyScheduleQueueName%")]
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
                    var events = _mapper.Map<IEnumerable<Event>>(scheduledStreams);
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
                logger.LogError($"{nameof(NotifyWeeklySchedule)} execution failed: {e.Message}");
                throw;
            }
        }
    }
}
