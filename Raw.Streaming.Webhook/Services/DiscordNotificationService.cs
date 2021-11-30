using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Exceptions;
using Raw.Streaming.Webhook.Model.Discord;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public class DiscordNotificationService : IDiscordNotificationService
    {
        private readonly HttpClient _client;
        private readonly ILogger<DiscordNotificationService> _logger;
        private readonly string _discordWebhookUrl = AppSettings.DiscordWebhookUrl;

        public DiscordNotificationService(HttpClient httpClient, ILogger<DiscordNotificationService> logger)
        {
            _client = httpClient;
            _logger = logger;
        }

        public async Task SendNotification(string webhookId, string webhookToken, Notification notification)
        {
            var fullUrl = $"{_discordWebhookUrl}/{webhookId}/{webhookToken}";
            var notificationRequestJson = JsonSerializer.Serialize(notification);
            var request = new HttpRequestMessage(HttpMethod.Post, fullUrl)
            {
                Content = new StringContent(notificationRequestJson, Encoding.UTF8, "application/json")
            };

            _logger.LogInformation($"Calling discord webhook endpoint with content:\n{notificationRequestJson}");
            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new DiscordApiException($"Error sending discord webhook request: {await response.Content.ReadAsStringAsync()}");
            }
        }
    }
}