using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Exceptions;
using Raw.Streaming.Webhook.Model.Twitch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Raw.Streaming.Webhook.Services
{
    internal class TwitchApiService : ITwitchApiService
    {
        private readonly string _twitchApiUrl = AppSettings.TwitchApiUrl;
        private readonly string _clientId = AppSettings.TwitchClientId;
        private readonly string _channelEndpoint = AppSettings.TwitchApiChannelEndpoint;
        private readonly string _clipEndpoint = AppSettings.TwitchApiClipEndpoint;
        private readonly string _gameEndpoint = AppSettings.TwitchApiGameEndpoint;
        private readonly string _scheduleEndpoint = AppSettings.TwitchApiScheduleEndpoint;
        private readonly string _videoEndpoint = AppSettings.TwitchApiVideoEndpoint;

        private readonly HttpClient _client;
        private readonly ITwitchTokenService _twitchTokenService;
        private readonly ILogger<TwitchApiService> _logger;

        public TwitchApiService(ILogger<TwitchApiService> logger, ITwitchTokenService twitchTokenService, HttpClient httpClient)
        {
            _logger = logger;
            _twitchTokenService = twitchTokenService;
            _client = httpClient;
        }

        public async Task<TwitchChannel> GetChannelInfoAsync(string broadcasterId)
        {
            try
            {
                var queryString = $"?broadcaster_id={broadcasterId}";
                var scope = "user:read:broadcast";
                _logger.LogDebug($"Calling twitch channel info endpoint with query string: {queryString}");
                var response = await SendTwitchApiGetRequestAsync<IList<TwitchChannel>>(_channelEndpoint, queryString, scope);
                return response.First();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Could not get channel info for channel id {broadcasterId}");
                throw;
            }
        }

        public async Task<IList<TwitchGame>> GetGamesAsync(params string[] gameIds)
        {
            var gameIdList = string.Join(',', gameIds);
            var queryString = $"?id={gameIdList}";
            var scope = "user:read:broadcast";
            _logger.LogDebug($"Calling twitch game endpoint with query string: {queryString}");
            return await SendTwitchApiGetRequestAsync<IList<TwitchGame>>(_gameEndpoint, queryString, scope);
        }

        public async Task<IList<TwitchClip>> GetClipsByBroadcasterAsync(string broadcasterId, DateTimeOffset? startedAt = null, DateTimeOffset? endedAt = null)
        {
            var queryString = $"?broadcaster_id={broadcasterId}";
            queryString = startedAt.HasValue ? $"{queryString}&started_at={UrlEncodeDateTime(startedAt)}" : queryString;
            queryString = endedAt.HasValue ? $"{queryString}&ended_at={UrlEncodeDateTime(endedAt)}" : queryString;
            var scope = "user:read:broadcast";
            _logger.LogDebug($"Calling twitch clip endpoint with query string: {queryString}");
            return await SendTwitchApiGetRequestAsync<IList<TwitchClip>>(_clipEndpoint, queryString, scope);
        }

        public async Task<IList<TwitchVideo>> GetHighlightsByBroadcasterAsync(string broadcasterId)
        {
            var queryString = $"?type=highlight&user_id={broadcasterId}";
            var scope = "user:read:broadcast";
            _logger.LogDebug($"Calling twitch clip endpoint with query string: {queryString}");
            return await SendTwitchApiGetRequestAsync<IList<TwitchVideo>>(_videoEndpoint, queryString, scope);
        }

        public async Task<TwitchSchedule> GetScheduleByBroadcasterIdAsync(string broadcasterId, DateTimeOffset? startTime = null)
        {
            var queryString = $"?broadcaster_id={broadcasterId}";
            queryString = startTime.HasValue ? $"{queryString}&start_time={UrlEncodeDateTime(startTime)}" : queryString;
            var scope = "user:read:broadcast";
            _logger.LogDebug($"Calling twitch clip endpoint with query string: {queryString}");
            return await SendTwitchApiGetRequestAsync<TwitchSchedule>(_scheduleEndpoint, queryString, scope);
        }

        private async Task<T> SendTwitchApiGetRequestAsync<T>(string endpoint, string queryString, string scope)
        {
            var fullUrl = $"{_twitchApiUrl}/{endpoint}{queryString}";
            var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            request.Headers.Add("Authorization", $"Bearer {await _twitchTokenService.GetTwitchTokenAsync(scope)}");
            request.Headers.Add("client-id", _clientId);

            var response = await _client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error calling twitch API endpoint: {fullUrl}");
                throw new TwitchApiException($"Error calling twitch API endpoint: {responseContent}");
            }

            var responseObject = JsonSerializer.Deserialize<TwitchApiResponse<T>>(responseContent);
            return responseObject.Data;
        }

        private static string UrlEncodeDateTime(DateTimeOffset? dateTime)
        {
            return HttpUtility.UrlEncode($"{dateTime?.UtcDateTime:yyyy-MM-ddTHH:mm:ssK}");
        }
    }
}
