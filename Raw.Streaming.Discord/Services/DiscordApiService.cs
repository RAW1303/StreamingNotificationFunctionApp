﻿using Microsoft.Extensions.Logging;
using Raw.Streaming.Discord.Exceptions;
using Raw.Streaming.Discord.Model.DiscordApi;
using Raw.Streaming.Discord.Model.DiscordApi.Exceptions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services;
internal class DiscordApiService : IDiscordApiService
{
    private readonly string _discordBotToken = AppSettings.DiscordBotToken;
    private readonly string _discordApiUrl = AppSettings.DiscordApiUrl;
    private readonly HttpClient _client;
    private readonly ILogger _logger;

    public DiscordApiService(HttpClient httpClient, ILogger<DiscordApiService> logger)
    {
        _client = httpClient;
        _logger = logger;
    }

    public async Task<T> SendDiscordApiGetRequestAsync<T>(string endpoint)
    {
        var response = await SendDiscordApiRequestAsync(HttpMethod.Get, endpoint);
        var responseObject = JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
        return responseObject;
    }

    public async Task<T> SendDiscordApiPostRequestAsync<T>(string endpoint, DiscordApiContent content = null)
    {
        var response = await SendDiscordApiRequestAsync(HttpMethod.Post, endpoint, content);
        var responseObject = JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
        return responseObject;
    }

    public async Task<T> SendDiscordApiPatchRequestAsync<T>(string endpoint, DiscordApiContent content = null)
    {
        var response = await SendDiscordApiRequestAsync(HttpMethod.Patch, endpoint, content);
        var responseObject = JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
        return responseObject;
    }

    public async Task SendDiscordApiDeleteRequestAsync(string endpoint)
    {
        await SendDiscordApiRequestAsync(HttpMethod.Delete, endpoint);
    }

    private async Task<HttpResponseMessage> SendDiscordApiRequestAsync(HttpMethod method, string endpoint, DiscordApiContent content = null)
    {
        var fullUrl = $"{_discordApiUrl}/{endpoint}";
        var request = new HttpRequestMessage(method, fullUrl);
        request.Headers.Add("Authorization", $"Bot {_discordBotToken}");

        if (content != null)
        {
            var notificationRequestJson = JsonSerializer.Serialize(content, content.GetType());
            request.Content = new StringContent(notificationRequestJson, Encoding.UTF8, "application/json");
        }

        var response = await _client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var errorMessage = $"{response.StatusCode} when error during {method.Method} request to Discord API endpoint {endpoint}\n{responseContent}";
            _logger.LogError(errorMessage);
            switch(response.StatusCode)
            {
                case HttpStatusCode.TooManyRequests:
                    throw new DiscordApiRateLimitException(errorMessage);
                default:
                    throw new DiscordApiException(errorMessage);
            }
        }

        return response;
    }
}
