using Microsoft.Extensions.Logging;
using Raw.Streaming.Discord.Exceptions;
using Raw.Streaming.Discord.Model.DiscordApi;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services
{
    internal abstract class BaseDiscordApiService
    {
        private readonly string _discordBotToken = AppSettings.DiscordBotToken;
        private readonly string _discordApiUrl = AppSettings.DiscordApiUrl;
        private readonly HttpClient _client;
        protected readonly ILogger _logger;

        protected BaseDiscordApiService(ILogger logger, HttpClient httpClient)
        {
            _client = httpClient;
            _logger = logger;
        }

        protected async Task<HttpResponseMessage> SendDiscordApiGetRequestAsync(string endpoint)
        {
            return await SendDiscordApiRequestAsync(HttpMethod.Get, endpoint);
        }

        protected async Task<HttpResponseMessage> SendDiscordApiPostRequestAsync(string endpoint, DiscordApiContent content = null)
        {
            return await SendDiscordApiRequestAsync(HttpMethod.Post, endpoint, content);
        }

        protected async Task<HttpResponseMessage> SendDiscordApiPatchRequestAsync(string endpoint, DiscordApiContent content = null)
        {
            return await SendDiscordApiRequestAsync(HttpMethod.Patch, endpoint, content);
        }

        protected async Task<HttpResponseMessage> SendDiscordApiDeleteRequestAsync(string endpoint)
        {
            return await SendDiscordApiRequestAsync(HttpMethod.Delete, endpoint);
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
                throw new DiscordApiException(errorMessage);
            }

            return response;
        }
    }
}
