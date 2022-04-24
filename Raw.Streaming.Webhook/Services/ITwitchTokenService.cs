using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    internal interface ITwitchTokenService
    {
        Task<string> GetTwitchTokenAsync(string scope);
    }
}
