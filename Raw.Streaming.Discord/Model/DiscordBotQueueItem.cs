using Raw.Streaming.Common.Model.Enums;
using Raw.Streaming.Discord.Model.DiscordApi;

namespace Raw.Streaming.Discord.Model
{
    public class DiscordBotQueueItem
    {
        public MessageType Type { get; set; }
        public Message[] Messages { get; set; }

        public DiscordBotQueueItem()
        {
        }

        public DiscordBotQueueItem(MessageType type, Message[] messages)
        {
            Type = type;
            Messages = messages;
        }
    }
}
