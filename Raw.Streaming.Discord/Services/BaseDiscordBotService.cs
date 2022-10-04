using Microsoft.Extensions.Logging;
using Raw.Streaming.Discord.Exceptions;
using Raw.Streaming.Discord.Model.DiscordApi;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services
{
    internal abstract class BaseDiscordBotService
    {
        private readonly string _discordBotToken = AppSettings.DiscordBotToken;
        private readonly string _discordApiUrl = AppSettings.DiscordApiUrl;
        private readonly HttpClient _client;
        protected readonly ILogger _logger;

        protected BaseDiscordBotService(ILogger logger, HttpClient httpClient)
        {
            _client = httpClient;
            _logger = logger;
        }

        protected async Task<HttpResponseMessage> SendDiscordApiGetRequestAsync(string endpoint)
        {
            return await SendDiscordApiRequestAsync(endpoint, null, HttpMethod.Get);
        }

        protected async Task<HttpResponseMessage> SendDiscordApiPostRequestAsync(string endpoint, DiscordApiContent content = null)
        {
            return await SendDiscordApiRequestAsync(endpoint, null, HttpMethod.Post, content);
        }

        protected async Task<HttpResponseMessage> SendDiscordApiPatchRequestAsync(string endpoint, DiscordApiContent content = null)
        {
            return await SendDiscordApiRequestAsync(endpoint, null, HttpMethod.Patch, content);
        }

        protected async Task<HttpResponseMessage> SendDiscordApiDeleteRequestAsync(string endpoint)
        {
            return await SendDiscordApiRequestAsync(endpoint, null, HttpMethod.Delete);
        }

        private async Task<HttpResponseMessage> SendDiscordApiRequestAsync(string endpoint, string queryString, HttpMethod method, DiscordApiContent content = null)
        {
            var fullUrl = $"{_discordApiUrl}/{endpoint}{queryString}";
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
