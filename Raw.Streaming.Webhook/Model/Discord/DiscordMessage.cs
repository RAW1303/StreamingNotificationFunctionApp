using Raw.Streaming.Common.Model.Enums;
using System.Collections.Generic;

namespace Raw.Streaming.Webhook.Model.Discord
{
    public class DiscordMessage
    {
        public MessageType Type { get; set; }
        public IEnumerable<Notification> Messages { get; set; }

        public DiscordMessage()
        {
        }

        public DiscordMessage(MessageType type, params Notification[] messages)
        {
            Type = type;
            Messages = messages;
        }
    }
}
