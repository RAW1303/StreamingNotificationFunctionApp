using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Raw.Streaming.Webhook.Model.Discord;
using Raw.Streaming.Webhook.Services;
using System;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Functions
{
    public class DiscordController
    {
        private readonly IDiscordNotificationService _discordNotificationService;

        public DiscordController(IDiscordNotificationService discordNotificationService)
        {
            _discordNotificationService = discordNotificationService;
        }

        [FunctionName("DiscordController")]
        public async Task Run([ServiceBusTrigger("discordnotificationqueue", Connection = "StreamingServiceBus")] string message, ILogger logger)
        {
            try
            {
                logger.LogInformation($"Discord notification started");
                var discordMessage = JsonConvert.DeserializeObject<DiscordMessage>(message);
                await _discordNotificationService.SendNotification(discordMessage.WebhookId, discordMessage.WebhookToken, discordMessage.Notification);
                logger.LogInformation($"Discord notification succeeded");
            }
            catch (Exception e)
            {
                logger.LogError($"Discord notification failed: {e.Message}\n{message}");
            }
        }
    }
}
