using Raw.Streaming.Common.Model.Enums;

namespace Raw.Streaming.Common.Model
{
    public class DiscordBotQueueItem
    {
        public MessageType Type { get; set; }
        public Entity[] Entities { get; set; }

        public DiscordBotQueueItem(MessageType type, params Entity[] entities)
        {
            Type = type;
            Entities = entities;
        }
    }
}
