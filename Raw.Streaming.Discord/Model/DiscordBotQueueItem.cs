using Raw.Streaming.Discord.Model.DiscordApi;

namespace Raw.Streaming.Discord.Model
{
    public class DiscordBotQueueItem
    {
        public string ChannelId { get; set; }
        public Message[] Messages { get; set; }

        public DiscordBotQueueItem()
        {
        }

        public DiscordBotQueueItem(string channelId, Message[] messages)
        {
            ChannelId = channelId;
            Messages = messages;
        }
    }
}
