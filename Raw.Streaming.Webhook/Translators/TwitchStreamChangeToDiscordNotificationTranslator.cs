using Raw.Streaming.Webhook.Common;
using Raw.Streaming.Webhook.Model.Discord;
using Raw.Streaming.Webhook.Model.Twitch;

namespace Raw.Streaming.Webhook.Translators
{
    public static class TwitchStreamChangeToDiscordNotificationTranslator
    {
        public static Notification Translate(Channel channel)
        {
            var lines = new string[] {
                $"{AppSettings.DiscordNotificationGroupIds} {channel.BroadcasterName} is live on Twitch!",
                $":pen: {channel.Title}",
                $":video-game: {channel.GameName}",
                $":tv: https://twitch.tv/{channel.BroadcasterName}"
            };

            return new Notification()
            {
                Content = string.Join("\n", lines)
            };
        }
    }
}
