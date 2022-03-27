using System.Collections.Generic;

namespace Raw.Streaming.Webhook.Model.Discord
{
    public class DiscordMessage
    {
        public string ChannelId { get; set; }
        public IEnumerable<Notification> Messages { get; set; }

        public DiscordMessage()
        {
        }

        public DiscordMessage(string channelId, params Notification[] messages)
        {
            ChannelId = channelId;
            Messages = messages;
        }
    }
}
