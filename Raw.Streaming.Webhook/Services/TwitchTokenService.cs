using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Exceptions;
using Raw.Streaming.Webhook.Model;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public class TwitchTokenService : ITwitchTokenService
    {
        private readonly string _twitchClientId = AppSettings.TwitchClientId;
        private readonly string _twitchClientSecret = AppSettings.TwitchClientSecret;
        private readonly string _twitchTokenUrl = AppSettings.TwitchTokenUrl;

        private readonly HttpClient _client;
        private readonly ILogger<TwitchTokenService> _logger;

        public TwitchTokenService(ILogger<TwitchTokenService> logger, HttpClient httpClient)
        {
            _client = httpClient;
            _logger = logger;
        }

        public async Task<string> GetTwitchTokenAsync(string scope)
        {
            _logger.LogDebug($"Getting Twitch token for clientID '{_twitchClientId}' with scope '{scope}'");
            var fullTokenUrl = $"{_twitchTokenUrl}?grant_type=client_credentials&scope={scope}&client_id={_twitchClientId}&client_secret={_twitchClientSecret}";
            var request = new HttpRequestMessage(HttpMethod.Post, fullTokenUrl);
            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error getting Twitch token for clientID '{_twitchClientId}' with scope '{scope}'");
                throw new TwitchApiException($"Error getting twitch token: {await response.Content.ReadAsStringAsync()}");
            }
            var responseString = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<TokenResponse>(responseString);
            return responseObject.AccessToken;
        }
    }
}
