using Microsoft.Extensions.Logging;
using Raw.Streaming.Discord.Exceptions;
using Raw.Streaming.Discord.Model.DiscordApi;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services;

internal abstract class DiscordApiService
{
    private static readonly string _discordBotToken = AppSettings.DiscordBotToken;
    private static readonly string _discordApiUrl = AppSettings.DiscordApiUrl;
    protected readonly HttpClient _client;
    protected readonly ILogger _logger;

    protected DiscordApiService(HttpClient httpClient, ILogger<DiscordApiService> logger)
    {
        _client = httpClient;
        _logger = logger;
    }

    protected async Task<T> SendDiscordApiGetRequestAsync<T>(string endpoint)
    {
        var response = await SendDiscordApiRequestAsync(HttpMethod.Get, endpoint);
        var responseObject = JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
        return responseObject;
    }

    protected async Task<T> SendDiscordApiPostRequestAsync<T>(string endpoint, DiscordApiContent? content = null)
    {
        var response = await SendDiscordApiRequestAsync(HttpMethod.Post, endpoint, content);
        var responseObject = JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
        return responseObject;
    }

    protected async Task<T> SendDiscordApiPatchRequestAsync<T>(string endpoint, DiscordApiContent? content = null)
    {
        var response = await SendDiscordApiRequestAsync(HttpMethod.Patch, endpoint, content);
        var responseObject = JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
        return responseObject;
    }

    protected async Task SendDiscordApiDeleteRequestAsync(string endpoint)
    {
        await SendDiscordApiRequestAsync(HttpMethod.Delete, endpoint);
    }

    private async Task<HttpResponseMessage> SendDiscordApiRequestAsync(HttpMethod method, string endpoint, DiscordApiContent? content = null)
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
            throw response.StatusCode switch
            {
                HttpStatusCode.TooManyRequests => new DiscordApiRateLimitException(errorMessage),
                _ => new DiscordApiException(errorMessage),
            };
        }

        return response;
    }
}
