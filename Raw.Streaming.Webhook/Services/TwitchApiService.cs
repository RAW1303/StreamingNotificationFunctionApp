using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Exceptions;
using Raw.Streaming.Webhook.Model.Twitch;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public class TwitchApiService : TwitchService, ITwitchApiService
    {
        private readonly string _twitchApiUrl = AppSettings.TwitchApiUrl;
        private readonly string _clientId = AppSettings.TwitchClientId;
        private readonly string _channelEndpoint = AppSettings.TwitchApiChannelEndpoint;
        private readonly string _gameEndpoint = AppSettings.TwitchApiGameEndpoint;
        private readonly string _clipEndpoint = AppSettings.TwitchApiClipEndpoint;
        private readonly string _videoEndpoint = AppSettings.TwitchApiVideoEndpoint;

        public TwitchApiService(ILogger<TwitchSubscriptionService> logger, HttpClient httpClient) : base(logger, httpClient)
        {
        }

        public async Task<TwitchChannel> GetChannelInfoAsync(string broadcasterId)
        {
            var queryString = $"?broadcaster_id={broadcasterId}";
            var scope = "user:read:broadcast";
            _logger.LogInformation($"Calling twitch channel info endpoint with query string: {queryString}");
            var response = await SendTwitchApiRequestAsync(_channelEndpoint, queryString, scope);
            var responseObject = JsonSerializer.Deserialize<TwitchApiResponse<TwitchChannel>>(await response.Content.ReadAsStringAsync());
            return responseObject.Data[0];
        }

        public async Task<IList<TwitchGame>> GetGamesAsync(params string[] gameIds)
        {
            var gameIdList = string.Join(',', gameIds);
            var queryString = $"?id={gameIdList}";
            var scope = "user:read:broadcast";
            _logger.LogInformation($"Calling twitch game endpoint with query string: {queryString}");
            var response = await SendTwitchApiRequestAsync(_gameEndpoint, queryString, scope);
            var responseObject = JsonSerializer.Deserialize<TwitchApiResponse<TwitchGame>>(await response.Content.ReadAsStringAsync());
            return responseObject.Data;
        }

        public async Task<IList<TwitchClip>> GetClipsByBroadcasterAsync(string broadcasterId, DateTime? startedAt = null, DateTime? endedAt = null)
        {
            var queryString = $"?broadcaster_id={broadcasterId}";
            queryString = startedAt.HasValue ? $"{queryString}&started_at={startedAt:yyyy-MM-ddTHH:mm:ssK}" : queryString;
            queryString = endedAt.HasValue ? $"{queryString}&ended_at={endedAt:yyyy-MM-ddTHH:mm:ssK}" : queryString;
            var scope = "user:read:broadcast";
            _logger.LogInformation($"Calling twitch clip endpoint with query string: {queryString}");
            var response = await SendTwitchApiRequestAsync(_clipEndpoint, queryString, scope);
            var responseObject = JsonSerializer.Deserialize<TwitchApiResponse<TwitchClip>>(await response.Content.ReadAsStringAsync());
            return responseObject.Data;
        }

        public async Task<IList<TwitchVideo>> GetHighlightsByBroadcasterAsync(string broadcasterId)
        {
            var queryString = $"?type=highlight&user_id={broadcasterId}";
            var scope = "user:read:broadcast";
            _logger.LogInformation($"Calling twitch clip endpoint with query string: {queryString}");
            var response = await SendTwitchApiRequestAsync(_videoEndpoint, queryString, scope);
            var responseObject = JsonSerializer.Deserialize<TwitchApiResponse<TwitchVideo>>(await response.Content.ReadAsStringAsync());
            return responseObject.Data;
        }

        private async Task<HttpResponseMessage> SendTwitchApiRequestAsync(string endpoint, string queryString, string scope)
        {
            var fullUrl = $"{_twitchApiUrl}/{endpoint}{queryString}";
            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            request.Headers.Add("Authorization", $"Bearer {await GetTwitchToken(scope)}");
            request.Headers.Add("client-id", _clientId);

            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error calling twitch API endpoint: {fullUrl}");
                throw new TwitchApiException($"Error calling twitch API endpoint: {await response.Content.ReadAsStringAsync()}");
            }
            return response;
        }
    }
}
