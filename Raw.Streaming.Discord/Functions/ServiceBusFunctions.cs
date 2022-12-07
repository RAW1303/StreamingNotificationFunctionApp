using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Services;
using Raw.Streaming.Discord.Translators;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Functions;

[ServiceBusAccount("StreamingServiceBus")]
internal class ServiceBusFunctions
{
    private readonly IDiscordEventService _discordEventService;
    private readonly IDiscordMessageService _discordMessageService;
    private readonly ILogger<ServiceBusFunctions> _logger;

    public ServiceBusFunctions(
        IDiscordEventService discordEventService,
        IDiscordMessageService discordMessageService, 
        ILogger<ServiceBusFunctions> logger)
    {
        _logger = logger;
        _discordEventService = discordEventService;
        _discordMessageService = discordMessageService;
    }

    [FunctionName(nameof(ProcessGoLiveMessageQueue))]
    public async Task ProcessGoLiveMessageQueue([ServiceBusTrigger("%GoLiveQueueName%")] DiscordBotQueueItem<GoLive> myQueueItem)
    {
        try
        {
            _logger.LogDebug($"{nameof(ProcessGoLiveMessageQueue)} notification started");
            var messages = myQueueItem.Entities.Select(x => GoLiveToDiscordMessageTranslator.Translate(x));
            await Task.WhenAll(messages.Select(x => _discordMessageService.SendDiscordMessageAsync(AppSettings.DiscordStreamGoLiveChannelId, x)));
            _logger.LogDebug($"{nameof(ProcessGoLiveMessageQueue)} notification succeeded");
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(ProcessGoLiveMessageQueue)} notification failed: {ex.Message}");
            throw;
        }
    }

    [FunctionName(nameof(ProcessClipMessageQueue))]
    public async Task ProcessClipMessageQueue([ServiceBusTrigger("%ClipsQueueName%")] DiscordBotQueueItem<Clip> myQueueItem)
    {
        try
        {
            _logger.LogDebug($"{nameof(ProcessClipMessageQueue)} notification started");
            var messages = myQueueItem.Entities.Select(x => ClipToDiscordMessageTranslator.Translate(x));
            await Task.WhenAll(messages.Select(x => _discordMessageService.SendDiscordMessageAsync(AppSettings.DiscordClipChannelId, x)));
            _logger.LogDebug($"{nameof(ProcessClipMessageQueue)} notification succeeded");
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(ProcessClipMessageQueue)} notification failed: {ex.Message}");
            throw;
        }
    }

    [FunctionName(nameof(ProcessVideoMessageQueue))]
    public async Task ProcessVideoMessageQueue([ServiceBusTrigger("%VideosQueueName%")] DiscordBotQueueItem<Video> myQueueItem)
    {
        try
        {
            _logger.LogDebug($"{nameof(ProcessVideoMessageQueue)} notification started");
            var messages = myQueueItem.Entities.Select(x => VideoToDiscordMessageTranslator.Translate(x));
            await Task.WhenAll(messages.Select(x => _discordMessageService.SendDiscordMessageAsync(AppSettings.DiscordVideoChannelId, x)));
            _logger.LogDebug($"{nameof(ProcessVideoMessageQueue)} notification succeeded");
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(ProcessVideoMessageQueue)} notification failed: {ex.Message}");
            throw;
        }
    }

    [FunctionName(nameof(ProcessEventMessageQueue))]
    public async Task ProcessEventMessageQueue([ServiceBusTrigger("%EventScheduleQueueName%")] DiscordBotQueueItem<Event> myQueueItem)
    {
        try
        {
            _logger.LogDebug($"{nameof(ProcessEventMessageQueue)} notification started");
            await _discordEventService.SyncScheduledEventsAsync(AppSettings.DiscordGuildId, myQueueItem.Entities);
            _logger.LogDebug($"{nameof(ProcessEventMessageQueue)} notification succeeded");
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(ProcessEventMessageQueue)} notification failed: {ex.Message}");
            throw;
        }
    }
}
