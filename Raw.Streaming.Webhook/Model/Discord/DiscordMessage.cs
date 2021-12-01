namespace Raw.Streaming.Webhook.Model.Discord
{
    public class DiscordMessage
    {
        public string WebhookId { get; set;  }
        public string WebhookToken { get; set; }
        public Notification Notification { get; set; }

        public DiscordMessage()
        {
        }

        public DiscordMessage(string webhookId, string webhookToken, Notification discordNotification)
        {
            WebhookId = webhookId;
            WebhookToken = webhookToken;
            Notification = discordNotification;
        }
    }
}
