using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Exceptions;
using Raw.Streaming.Webhook.Model;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public abstract class TwitchService
    {
        protected readonly HttpClient _client;
        protected readonly ILogger<TwitchSubscriptionService> _logger;

        protected TwitchService(ILogger<TwitchSubscriptionService> logger, HttpClient httpClient)
        {
            _client = httpClient;
            _logger = logger;
        }

        protected async Task<string> GetTwitchToken(string scope)
        {
            var fullTokenUrl = $"{AppSettings.TwitchTokenUrl}?grant_type=client_credentials&scope={scope}&client_id={AppSettings.TwitchClientId}&client_secret={AppSettings.TwitchClientSecret}";
            var request = new HttpRequestMessage(HttpMethod.Post, fullTokenUrl);
            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new TwitchApiException($"Error getting twitch token: {await response.Content.ReadAsStringAsync()}");
            }
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<TokenResponse>(responseString);
            return responseObject.AccessToken;
        }
    }
}
