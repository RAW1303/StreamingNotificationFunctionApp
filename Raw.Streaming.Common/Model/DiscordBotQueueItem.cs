using Raw.Streaming.Common.Model.Enums;

namespace Raw.Streaming.Common.Model
{
    public class DiscordBotQueueItem<T> where T : Entity
    {
        public MessageType Type { get; set; }
        public T[] Entities { get; set; }

        public DiscordBotQueueItem()
        {
        }

        public DiscordBotQueueItem(MessageType type, params T[] entities)
        {
            Type = type;
            Entities = entities;
        }
    }
}
