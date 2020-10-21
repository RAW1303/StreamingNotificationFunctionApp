using Raw.Streaming.Webhook.Model;
using System.Threading.Tasks;

namespace Raw.Streaming.Webhook.Services
{
    public interface ITwitchApiService
    {
        Task<TwitchGame[]> GetGames(params string[] gameId);
    }
}
