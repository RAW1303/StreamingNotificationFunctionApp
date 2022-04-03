using Raw.Streaming.Webhook.Model.Discord;
using Raw.Streaming.Webhook.Model.Youtube;

namespace Raw.Streaming.Webhook.Translators
{
    public static class YoutubeFeedToDiscordNotificationTranslator
    {
        public static Notification Translate(YoutubeFeed youtubeFeed)
        {
            return new Notification()
            {
                Content = $"{youtubeFeed.Link}"
            };
        }
    }
}
