using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Exceptions;
using Raw.Streaming.Webhook.Model;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public class TwitchApiService : TwitchService, ITwitchApiService
    {
        public TwitchApiService(ILogger<TwitchSubscriptionService> logger, HttpClient httpClient) : base(logger, httpClient)
        {
        }

        public async Task<TwitchGame[]> GetGames(params string[] gameIds)
        {
            var gameIdList = string.Join(',', gameIds);
            var fullUrl = $"{AppSettings.TwitchApiUrl}/{AppSettings.TwitchApiGameEndpoint}?id={gameIdList}";

            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);

            var scope = "user:read:broadcast";
            request.Headers.Add("Authorization", $"Bearer {await GetTwitchToken(scope)}");
            request.Headers.Add("client-id", AppSettings.TwitchClientId);

            _logger.LogInformation($"Calling twitch game endpoint with ids: {gameIdList}");
            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new TwitchApiException($"Error while subscribing: {await response.Content.ReadAsStringAsync()}");
            }

            var responseObject = JsonSerializer.Deserialize<TwitchGameResponse>(await response.Content.ReadAsStringAsync());
            return responseObject.Data;
        }
    }
}
