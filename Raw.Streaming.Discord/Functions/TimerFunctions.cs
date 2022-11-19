using Microsoft.Azure.WebJobs;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Discord.Services;
using System.Linq;
using Raw.Streaming.Discord.Translators;

namespace Raw.Streaming.Discord.Functions;

internal class TimerFunctions
{
    private readonly IDiscordEventService _discordEventService;
    private readonly IDiscordMessageService _discordMessageService;
    private readonly ILogger<TimerFunctions> _logger;

    public TimerFunctions(
        IDiscordEventService discordEventService,
        IDiscordMessageService discordMessageService,
        ILogger<TimerFunctions> logger)
    {
        _logger = logger;
        _discordEventService = discordEventService;
        _discordMessageService = discordMessageService;
    }

    [ExcludeFromCodeCoverage]
    [FunctionName(nameof(NotifyDailyScheduleTrigger))]
    public async Task NotifyDailyScheduleTrigger(
        [TimerTrigger("%DailyScheduleTimerTrigger%")] TimerInfo timer)
    {
        await NotifyDailySchedule(DateTimeOffset.UtcNow);
    }

    public async Task NotifyDailySchedule(DateTimeOffset triggerTime)
    {
        try
        {
            _logger.LogDebug($"{nameof(NotifyDailySchedule)} execution started for {triggerTime}");
            var to = triggerTime.Date.AddDays(1);
            var events = await _discordEventService.GetScheduledEvents(AppSettings.DiscordGuildId);
            var todaysEvents = events.Where(x => x.ScheduledStartTime < to);
            _logger.LogDebug($"Found {todaysEvents.Count()} events for {to}");
            if (todaysEvents.Any())
            {
                var message = SheduledEventToDiscordMessageTranslator.TranslateDailySchedule(todaysEvents);
                await _discordMessageService.SendDiscordMessageAsync(AppSettings.DiscordScheduleChannelId, message);
            }
            _logger.LogDebug($"{nameof(NotifyDailySchedule)} execution complete for {triggerTime}");
        }
        catch (Exception e)
        {
            _logger.LogError($"{nameof(NotifyDailySchedule)} execution failed: {e.Message}");
            throw;
        }
    }
}
