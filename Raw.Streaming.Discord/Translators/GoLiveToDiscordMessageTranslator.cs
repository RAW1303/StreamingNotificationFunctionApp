using Raw.Streaming.Common.Model;
using Raw.Streaming.Discord.Model.DiscordApi;

namespace Raw.Streaming.Discord.Translators
{
    internal static class GoLiveToDiscordMessageTranslator
    {
        public static Message Translate(GoLive goLive)
        {
            var lines = new string[] {
                $"{AppSettings.DiscordNotificationGroupIds} {goLive.BroadcasterName} is live on Twitch!",
                $":pencil2: {goLive.Title}",
                $":video_game: {goLive.GameName}",
                $":tv: {goLive.Url}"
            };

            return new Message()
            {
                Content = string.Join("\n", lines)
            };
        }
    }
}
