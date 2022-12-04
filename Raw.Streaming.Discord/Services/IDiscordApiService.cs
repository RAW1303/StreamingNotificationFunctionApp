using Raw.Streaming.Discord.Model.DiscordApi;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services;
internal interface IDiscordApiService
{
    Task<T> SendDiscordApiGetRequestAsync<T>(string endpoint);
    Task<T> SendDiscordApiPostRequestAsync<T>(string endpoint, DiscordApiContent? content = null);
    Task<T> SendDiscordApiPatchRequestAsync<T>(string endpoint, DiscordApiContent? content = null);
    Task SendDiscordApiDeleteRequestAsync(string endpoint);
}
