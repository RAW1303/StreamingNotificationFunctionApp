using Raw.Streaming.Discord.Model.DiscordApi;
using System.Threading.Tasks;

namespace Raw.Streaming.Discord.Services
{
    internal interface IDiscordBotMessageService
    {
        public Task<Message> SendDiscordMessageAsync(string channelId, Message message);
        public Task<Message> CrosspostDiscordMessageAsync(string channelId, string messageId);
    }
}
