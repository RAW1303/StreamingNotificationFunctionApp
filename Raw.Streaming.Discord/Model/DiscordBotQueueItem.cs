using Raw.Streaming.Discord.Model.DiscordApi;

namespace Raw.Streaming.Discord.Model
{
    public class DiscordBotQueueItem
    {
        public string WebhookId { get; set; }
        public string WebhookToken { get; set; }
        public Message Notification { get; set; }

        public DiscordBotQueueItem()
        {
        }

        public DiscordBotQueueItem(string webhookId, string webhookToken, Message notification)
        {
            WebhookId = webhookId;
            WebhookToken = webhookToken;
            Notification = notification;
        }
    }
}
