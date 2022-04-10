using Raw.Streaming.Common.Model.Enums;
using System.Text.Json.Serialization;

namespace Raw.Streaming.Common.Model
{
    public class DiscordBotQueueItem
    {
        [JsonInclude]
        public MessageType Type { get; set; }
        [JsonInclude]
        public Entity[] Entities { get; set; }

        public DiscordBotQueueItem(MessageType type, params Entity[] entities)
        {
            Type = type;
            Entities = entities;
        }
    }
}
