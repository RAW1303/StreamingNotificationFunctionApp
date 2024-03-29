﻿using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Raw.Streaming.Webhook.Exceptions;
using Raw.Streaming.Webhook.Model.Twitch.EventSub;

namespace Raw.Streaming.Webhook.Services
{
    internal class TwitchSubscriptionService : ITwitchSubscriptionService
    {
        private readonly string _secret = AppSettings.TwitchSubscriptionSecret;

        private readonly HttpClient _client;
        private readonly ITwitchTokenService _twitchTokenService;
        private readonly ILogger<TwitchSubscriptionService> _logger;

        public TwitchSubscriptionService(ILogger<TwitchSubscriptionService> logger, ITwitchTokenService twitchTokenService, HttpClient httpClient)
        {
            _logger = logger;
            _twitchTokenService = twitchTokenService;
            _client = httpClient;
        }

        public async Task SubscribeAsync<T>(string type, T condition, string callbackUrl) where T: Condition
        {
            var subscriptionRequest = GenerateSubscriptionRequest(type, condition, callbackUrl);
            var subscriptionRequestJson = JsonSerializer.Serialize(subscriptionRequest);
            var request = new HttpRequestMessage(HttpMethod.Post, AppSettings.TwitchSubscriptionUrl)
            {
                Content = new StringContent(subscriptionRequestJson, Encoding.UTF8, "application/json")
            };

            var scope = "user:read:broadcast";
            request.Headers.Add("Authorization", $"Bearer {await _twitchTokenService.GetTwitchTokenAsync(scope)}");
            request.Headers.Add("client-id", AppSettings.TwitchClientId);

            _logger.LogDebug($"Calling twitch subscription endpoint with content:\n{subscriptionRequestJson}");
            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new TwitchApiException($"Error while subscribing:\n{subscriptionRequestJson}\n{await response.Content.ReadAsStringAsync()}");
            }
        }

        public EventSubRequest<T> GenerateSubscriptionRequest<T>(string type, T condition, string callbackUrl) where T : Condition
        {
            return new EventSubRequest<T>
            {
                Type = type,
                Version = "1",
                Condition = condition,
                Transport = new Transport
                {
                    Method = "webhook",
                    Callback = callbackUrl,
                    Secret = _secret
                }
            };
        }
    }
}
